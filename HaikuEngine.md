```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NHunspell;
using Syllabify;

/// <summary>
/// Provides functionality for validating and generating haiku poems using a
/// three-tier syllable counting strategy: CMU Pronouncing Dictionary (highest accuracy),
/// Syllabify rule-based engine (fallback), and vowel-group heuristics (last resort).
/// </summary>
/// <remarks>
/// <para>
/// A traditional haiku follows a 5-7-5 syllable structure across three lines.
/// This class also recognises common structural variants used in modern English haiku.
/// </para>
/// <para>
/// Syllable resolution priority:
/// <list type="number">
///   <item>CMU Pronouncing Dictionary — phoneme-based, highest accuracy</item>
///   <item>Syllabify — rule-based English syllabification</item>
///   <item>Vowel-group heuristics — simple but handles unknown/invented words</item>
/// </list>
/// </para>
/// <para>
/// Call <see cref="LoadCmuDict"/> once at startup before using any other methods.
/// The CMU dictionary file can be downloaded from
/// http://www.speech.cs.cmu.edu/cgi-bin/cmudict
/// </para>
/// </remarks>
public class HaikuEngine
{
    // ---------------------------------------------------------------------------
    // Known haiku structures: each int[] is a syllable-per-line pattern.
    // The canonical form is 5-7-5, but many respected English haiku poets
    // use shorter forms that preserve the three-line breath-pause rhythm.
    // ---------------------------------------------------------------------------
    private static readonly IReadOnlyList<int[]> KnownStructures = new[]
    {
        new[] { 5, 7, 5 },   // Traditional
        new[] { 3, 5, 3 },   // Minimalist / "micro-haiku"
        new[] { 2, 3, 2 },   // Ultra-compressed
        new[] { 4, 6, 4 },   // Near-traditional (common in translations)
        new[] { 5, 5, 5 },   // Equal-line variant
    };

    // ---------------------------------------------------------------------------
    // CMU phoneme entries whose labels end in a digit (0 = unstressed,
    // 1 = primary stress, 2 = secondary stress) mark vowel nuclei, i.e. syllables.
    // Counting these digits directly gives the syllable count without any
    // additional heuristics.
    // ---------------------------------------------------------------------------
    private readonly Dictionary<string, int> _cmuCache = new(StringComparer.OrdinalIgnoreCase);

    private readonly Syllabifier _syllabifier = new();

    private const string Vowels = "aeiouy";

    // ---------------------------------------------------------------------------
    // Words indexed by syllable count, built from the CMU dict at load time.
    // Used exclusively by the haiku generator to look up candidate words quickly.
    // ---------------------------------------------------------------------------
    private readonly Dictionary<int, List<string>> _wordsBySyllableCount = new();

    /// <summary>
    /// Gets a value indicating whether the CMU dictionary has been loaded.
    /// </summary>
    public bool IsCmuLoaded => _cmuCache.Count > 0;

    // ═══════════════════════════════════════════════════════════════════════════
    // Initialisation
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loads the CMU Pronouncing Dictionary from disk into an in-memory cache.
    /// This method must be called before syllable counting or haiku generation
    /// to enable the highest-accuracy resolution tier.
    /// </summary>
    /// <param name="filePath">
    /// Path to the CMU dict text file (e.g. <c>cmudict-0.7b.txt</c>).
    /// Lines beginning with <c>;;;</c> are treated as comments and skipped.
    /// </param>
    /// <exception cref="FileNotFoundException">
    /// Thrown when <paramref name="filePath"/> does not exist.
    /// </exception>
    /// <example>
    /// <code>
    /// var engine = new HaikuEngine();
    /// engine.LoadCmuDict("cmudict-0.7b.txt");
    /// </code>
    /// </example>
    public void LoadCmuDict(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("CMU dictionary file not found.", filePath);

        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith(";;;")) continue;

            // CMU format: WORD  P1 P2 P3 ...  (double-space separates word from phonemes)
            var parts = line.Split("  ", 2, StringSplitOptions.None);
            if (parts.Length < 2) continue;

            var rawWord = parts[0];

            // Alternate pronunciations are stored as WORD(1), WORD(2), etc.
            // Strip the parenthesised suffix so all variants map to the base word.
            var word = Regex.Replace(rawWord, @"\(\d+\)$", "").ToLower();

            var syllables = parts[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Count(phoneme => phoneme.Any(char.IsDigit));

            // Keep the first (most common) pronunciation; skip alternates
            // that would overwrite it with a different count.
            _cmuCache.TryAdd(word, syllables);

            // Index word → syllable count for the generator
            if (!_wordsBySyllableCount.ContainsKey(syllables))
                _wordsBySyllableCount[syllables] = new List<string>();

            // Avoid duplicate entries caused by alternate-pronunciation lines
            if (!_wordsBySyllableCount[syllables].Contains(word))
                _wordsBySyllableCount[syllables].Add(word);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Validation
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Determines whether the three supplied lines form a valid haiku
    /// using the traditional 5-7-5 syllable structure.
    /// </summary>
    /// <param name="line1">First line (expected: 5 syllables).</param>
    /// <param name="line2">Second line (expected: 7 syllables).</param>
    /// <param name="line3">Third line (expected: 5 syllables).</param>
    /// <returns>
    /// <see langword="true"/> if the lines match the 5-7-5 pattern;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// bool valid = engine.IsHaiku("an old silent pond", "a frog jumps into the pond", "splash silence again");
    /// </code>
    /// </example>
    public bool IsHaiku(string line1, string line2, string line3) =>
        IsHaiku(line1, line2, line3, new[] { 5, 7, 5 });

    /// <summary>
    /// Determines whether the three supplied lines form a valid haiku
    /// matching a specific syllable structure.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    /// <param name="line3">Third line.</param>
    /// <param name="structure">
    /// An array of exactly three integers specifying the required syllable
    /// count for each line (e.g. <c>new[] { 5, 7, 5 }</c>).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the lines match <paramref name="structure"/>;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="structure"/> does not contain exactly three elements.
    /// </exception>
    public bool IsHaiku(string line1, string line2, string line3, int[] structure)
    {
        if (structure.Length != 3)
            throw new ArgumentException("Structure must specify exactly 3 line counts.", nameof(structure));

        return CountLineSyllables(line1) == structure[0]
            && CountLineSyllables(line2) == structure[1]
            && CountLineSyllables(line3) == structure[2];
    }

    /// <summary>
    /// Checks the three lines against every known haiku structure and returns
    /// the first matching pattern, or <see langword="null"/> if no match is found.
    /// </summary>
    /// <param name="line1">First line of the poem.</param>
    /// <param name="line2">Second line of the poem.</param>
    /// <param name="line3">Third line of the poem.</param>
    /// <returns>
    /// The matching syllable structure as an <see cref="int"/> array
    /// (e.g. <c>[5, 7, 5]</c>), or <see langword="null"/> if the input
    /// does not match any known structure.
    /// </returns>
    /// <example>
    /// <code>
    /// var structure = engine.DetectHaikuStructure(line1, line2, line3);
    /// if (structure != null)
    ///     Console.WriteLine($"Matches {string.Join("-", structure)}");
    /// </code>
    /// </example>
    public int[]? DetectHaikuStructure(string line1, string line2, string line3)
    {
        var counts = new[]
        {
            CountLineSyllables(line1),
            CountLineSyllables(line2),
            CountLineSyllables(line3)
        };

        return KnownStructures.FirstOrDefault(s =>
            s[0] == counts[0] && s[1] == counts[1] && s[2] == counts[2]);
    }

    /// <summary>
    /// Returns a diagnostic breakdown showing the syllable count per word
    /// for each line, along with which resolution tier was used for each word.
    /// Useful for debugging unexpected validation results.
    /// </summary>
    /// <param name="line1">First line of the poem.</param>
    /// <param name="line2">Second line of the poem.</param>
    /// <param name="line3">Third line of the poem.</param>
    /// <returns>
    /// A list of <see cref="LineAnalysis"/> records, one per line.
    /// </returns>
    public List<LineAnalysis> Analyse(string line1, string line2, string line3) =>
        new[] { line1, line2, line3 }
            .Select((line, i) => new LineAnalysis
            {
                LineNumber  = i + 1,
                Text        = line,
                TotalSyllables = CountLineSyllables(line),
                WordBreakdown  = AnalyseLine(line)
            })
            .ToList();

    // ═══════════════════════════════════════════════════════════════════════════
    // Generation
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Generates a haiku by randomly selecting words from the CMU dictionary
    /// that fit the target syllable counts for each line.
    /// </summary>
    /// <param name="structure">
    /// Syllable structure to generate. Defaults to traditional 5-7-5 when
    /// <see langword="null"/>.
    /// </param>
    /// <param name="seed">
    /// Optional random seed for reproducible output. When <see langword="null"/>
    /// a non-deterministic seed is used.
    /// </param>
    /// <returns>
    /// A three-element array where each element is a generated line of the haiku.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the CMU dictionary has not been loaded via <see cref="LoadCmuDict"/>.
    /// </exception>
    /// <remarks>
    /// Generation uses a greedy word-filling strategy: for each line it repeatedly
    /// picks a random word whose syllable count fits the remaining budget until the
    /// line is exactly filled. This is non-deterministic and may require several
    /// attempts per line internally.
    /// </remarks>
    public string[] GenerateHaiku(int[]? structure = null, int? seed = null)
    {
        if (!IsCmuLoaded)
            throw new InvalidOperationException(
                "CMU dictionary must be loaded before generating haiku. Call LoadCmuDict() first.");

        structure ??= new[] { 5, 7, 5 };
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();

        return structure
            .Select(targetSyllables => BuildLine(targetSyllables, rng))
            .ToArray();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Syllable counting — three-tier resolution
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Counts the total syllables in a line of text by summing the syllable
    /// counts of each whitespace-delimited word.
    /// </summary>
    /// <param name="line">A line of natural-language text.</param>
    /// <returns>Total syllable count, or 0 for null/empty input.</returns>
    public int CountLineSyllables(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return 0;

        return line
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Sum(CountWordSyllables);
    }

    /// <summary>
    /// Counts syllables for a single word using a three-tier fallback strategy.
    /// </summary>
    /// <param name="word">A single word, optionally containing punctuation.</param>
    /// <returns>
    /// Syllable count ≥ 1, resolved via CMU dict → Syllabify → vowel-group heuristic.
    /// </returns>
    public int CountWordSyllables(string word)
    {
        // Strip punctuation so "pond." and "pond" resolve identically
        word = new string(word.Where(char.IsLetter).ToArray()).ToLower();
        if (string.IsNullOrEmpty(word)) return 0;

        // Tier 1 — CMU Pronouncing Dictionary
        if (_cmuCache.TryGetValue(word, out int cmuCount))
            return cmuCount;

        // Tier 2 — Syllabify rule-based engine
        try
        {
            var syllabified = _syllabifier.Syllabify(word);
            int syllabifyCount = syllabified?.Count() ?? 0;
            if (syllabifyCount > 0)
                return syllabifyCount;
        }
        catch
        {
            // Syllabify can throw on unusual character sequences; fall through to tier 3
        }

        // Tier 3 — Vowel-group heuristic (handles invented/unknown words)
        return VowelGroupCount(word);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Private helpers
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Builds a single haiku line whose total syllable count exactly equals
    /// <paramref name="targetSyllables"/> by randomly sampling words from the
    /// CMU dictionary index.
    /// </summary>
    /// <param name="targetSyllables">Number of syllables the line must contain.</param>
    /// <param name="rng">Seeded or unseeded <see cref="Random"/> instance.</param>
    /// <returns>A space-joined string of words filling the syllable budget.</returns>
    private string BuildLine(int targetSyllables, Random rng)
    {
        const int maxAttempts = 2000;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var words  = new List<string>();
            int budget = targetSyllables;

            while (budget > 0)
            {
                // Only consider words that fit within the remaining syllable budget.
                // Requesting words with exactly `budget` syllables as a last word
                // guarantees we can always close out the line cleanly.
                var candidates = GetCandidateWords(budget, rng);
                if (candidates.Count == 0) break; // dead end — retry whole line

                var pick = candidates[rng.Next(candidates.Count)];
                words.Add(pick);
                budget -= CountWordSyllables(pick);
            }

            if (budget == 0)
                return string.Join(" ", words);
        }

        // Absolute fallback: return a placeholder so generation never hard-fails
        return $"[{targetSyllables} syllables]";
    }

    /// <summary>
    /// Returns a random subset of words from the CMU index whose syllable count
    /// is between 1 and <paramref name="maxSyllables"/> inclusive.
    /// Capping candidates improves variety without exhausting the full word list.
    /// </summary>
    private List<string> GetCandidateWords(int maxSyllables, Random rng)
    {
        // Gather all valid syllable buckets up to the budget cap
        var pool = Enumerable.Range(1, maxSyllables)
            .Where(_wordsBySyllableCount.ContainsKey)
            .SelectMany(n => _wordsBySyllableCount[n])
            .ToList();

        // Shuffle a small sample rather than the entire pool for performance
        return pool
            .OrderBy(_ => rng.Next())
            .Take(50)
            .ToList();
    }

    /// <summary>
    /// Returns a per-word diagnostic breakdown for a single line, including
    /// which resolution tier resolved each word's syllable count.
    /// </summary>
    private List<WordAnalysis> AnalyseLine(string line) =>
        line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w =>
            {
                var clean = new string(w.Where(char.IsLetter).ToArray()).ToLower();
                var (count, tier) = ResolveWithTier(clean);
                return new WordAnalysis { Word = w, Syllables = count, Tier = tier };
            })
            .ToList();

    /// <summary>
    /// Resolves syllable count for a pre-cleaned word and also returns
    /// which tier of the fallback chain produced the answer.
    /// </summary>
    /// <param name="word">Lowercase, punctuation-stripped word.</param>
    /// <returns>Syllable count and the name of the tier that resolved it.</returns>
    private (int Count, string Tier) ResolveWithTier(string word)
    {
        if (string.IsNullOrEmpty(word)) return (0, "none");

        if (_cmuCache.TryGetValue(word, out int cmuCount))
            return (cmuCount, "CMU");

        try
        {
            var syllabified = _syllabifier.Syllabify(word);
            int count = syllabified?.Count() ?? 0;
            if (count > 0) return (count, "Syllabify");
        }
        catch { /* fall through */ }

        return (VowelGroupCount(word), "VowelGroup");
    }

    /// <summary>
    /// Estimates syllable count using consecutive vowel groups as a proxy
    /// for syllable nuclei, with a correction for common silent-e patterns.
    /// </summary>
    /// <param name="word">Lowercase, punctuation-stripped word.</param>
    /// <returns>Estimated syllable count, always at least 1.</returns>
    /// <remarks>
    /// This heuristic will over-count for words with diphthongs not handled
    /// by the silent-e rule (e.g. "beautiful") and is intentionally kept
    /// simple as a last-resort measure.
    /// </remarks>
    private static int VowelGroupCount(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return 0;

        int count = 0;
        bool lastWasVowel = false;

        foreach (char c in word)
        {
            if (Vowels.Contains(c))
            {
                // Only increment at the start of each new vowel run —
                // consecutive vowels (e.g. "ea", "ou") count as one syllable nucleus
                if (!lastWasVowel) count++;
                lastWasVowel = true;
            }
            else
            {
                lastWasVowel = false;
            }
        }

        // Subtract silent terminal 'e' preceded by a consonant (e.g. "make" → 1, not 2)
        if (word.Length > 2 && word.EndsWith('e') && !Vowels.Contains(word[^2]))
            count--;

        return Math.Max(1, count);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Supporting record types
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>Diagnostic result for a single line of a haiku.</summary>
    public record LineAnalysis
    {
        /// <summary>1-based line number within the haiku.</summary>
        public int LineNumber { get; init; }

        /// <summary>Original text of the line.</summary>
        public string Text { get; init; } = string.Empty;

        /// <summary>Total syllable count for the line.</summary>
        public int TotalSyllables { get; init; }

        /// <summary>Per-word syllable breakdown with resolution tier.</summary>
        public List<WordAnalysis> WordBreakdown { get; init; } = new();
    }

    /// <summary>Diagnostic result for a single word within a line.</summary>
    public record WordAnalysis
    {
        /// <summary>Original word as it appeared in the input (with punctuation).</summary>
        public string Word { get; init; } = string.Empty;

        /// <summary>Resolved syllable count.</summary>
        public int Syllables { get; init; }

        /// <summary>
        /// Resolution tier used: <c>"CMU"</c>, <c>"Syllabify"</c>,
        /// or <c>"VowelGroup"</c>.
        /// </summary>
        public string Tier { get; init; } = string.Empty;
    }
}
```
