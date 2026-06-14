using System.Text.RegularExpressions;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Haiku;

/// <summary>
/// Core poetry engine: loads the CMU pronunciation dictionary, counts syllables,
/// validates poetic forms, detects poem types, generates poems, and analyzes rhyme.
/// </summary>
public partial class PoemEngine
{
    private static readonly IReadOnlyDictionary<PoemType, PoemDefinition> Definitions = new Dictionary<PoemType, PoemDefinition>
    {
        [PoemType.Haiku] = new()
        {
            Type = PoemType.Haiku,
            Name = "Haiku",
            Description = "A traditional Japanese form with 5-7-5 syllable pattern across three lines.",
            SyllablePattern = [5, 7, 5],
        },
        [PoemType.Monoku] = new()
        {
            Type = PoemType.Monoku,
            Name = "Monoku",
            Description = "A single-line poem where total syllables must be between 4 and 17 inclusive.",
            SyllablePattern = [0],
        },
        [PoemType.Tanka] = new()
        {
            Type = PoemType.Tanka,
            Name = "Tanka",
            Description = "A five-line Japanese form with 5-7-5-7-7 syllable pattern.",
            SyllablePattern = [5, 7, 5, 7, 7],
        },
        [PoemType.Katauta] = new()
        {
            Type = PoemType.Katauta,
            Name = "Katauta",
            Description = "A three-line classical Japanese form with 5-7-7 syllable pattern.",
            SyllablePattern = [5, 7, 7],
        },
        [PoemType.Sedoka] = new()
        {
            Type = PoemType.Sedoka,
            Name = "Sedoka",
            Description = "A six-line poem equivalent to two joined katauta (5-7-7-5-7-7).",
            SyllablePattern = [5, 7, 7, 5, 7, 7],
        },
        [PoemType.Choka] = new()
        {
            Type = PoemType.Choka,
            Name = "Choka",
            Description = "A long poem with alternating 5-7 syllable lines, ending with 5-7-7. Always an odd number of lines.",
            SyllablePattern = [],
        },
        [PoemType.AmericanLune] = new()
        {
            Type = PoemType.AmericanLune,
            Name = "American Lune",
            Description =
                "A three-line modern American adaptation of haiku with 3-5-3 syllable pattern. Formerly called Minimalist.",
            SyllablePattern = [3, 5, 3],
        },
        [PoemType.KellyLune] = new()
        {
            Type = PoemType.KellyLune,
            Name = "Kelly Lune",
            Description = "A three-line form created by Robert Kelly with 5-3-5 syllable pattern.",
            SyllablePattern = [5, 3, 5],
        },
        [PoemType.AmericanCinquain] = new()
        {
            Type = PoemType.AmericanCinquain,
            Name = "American Cinquain",
            Description = "A five-line poem with 2-4-6-8-2 syllable pattern, invented by Adelaide Crapsey.",
            SyllablePattern = [2, 4, 6, 8, 2],
        },
        [PoemType.ReverseCinquain] = new()
        {
            Type = PoemType.ReverseCinquain,
            Name = "Reverse Cinquain",
            Description = "A five-line poem with 2-8-6-4-2 syllable pattern, the reverse of the American cinquain.",
            SyllablePattern = [2, 8, 6, 4, 2],
        },
        [PoemType.MirrorCinquain] = new()
        {
            Type = PoemType.MirrorCinquain,
            Name = "Mirror Cinquain",
            Description =
                "A ten-line poem formed by concatenating an American cinquain and a Reverse cinquain (2-4-6-8-2-2-8-6-4-2).",
            SyllablePattern = [2, 4, 6, 8, 2, 2, 8, 6, 4, 2],
        },
        [PoemType.ButterflyCinquain] = new()
        {
            Type = PoemType.ButterflyCinquain,
            Name = "Butterfly Cinquain",
            Description =
                "A nine-line poem formed by merging an American cinquain with its reverse, dropping the center line (2-4-6-8-2-8-6-4-2).",
            SyllablePattern = [2, 4, 6, 8, 2, 8, 6, 4, 2],
        },
        [PoemType.Isosyllabic] = new()
        {
            Type = PoemType.Isosyllabic,
            Name = "Isosyllabic",
            Description = "A poem where every line has the same syllable count. Any number of lines n >= 2.",
            SyllablePattern = [],
        },
        [PoemType.Compressed] = new()
        {
            Type = PoemType.Compressed,
            Name = "Compressed",
            Description = "A three-line nonstandard haiku-inspired ultra-short form with 2-3-2 syllable pattern.",
            SyllablePattern = [2, 3, 2],
        },
        [PoemType.NearTraditional] = new()
        {
            Type = PoemType.NearTraditional,
            Name = "Near-Traditional",
            Description = "A three-line nonstandard approximation of haiku with 4-6-4 syllable pattern.",
            SyllablePattern = [4, 6, 4],
        },
        [PoemType.Freeform] = new()
        {
            Type = PoemType.Freeform,
            Name = "Freeform",
            Description = "A poem with no fixed syllable constraints.",
            SyllablePattern = [],
        },
    };

    private readonly Dictionary<string, (int SyllableCount, string[] Phonemes)> _cmuCache = new(
        StringComparer.OrdinalIgnoreCase
    );
    private readonly Dictionary<int, List<string>> _wordsBySyllableCount = [];
    private const string Vowels = "aeiouy";

    /// <summary>
    /// Gets a value indicating whether the CMU pronunciation dictionary has been loaded.
    /// </summary>
    /// <value><c>true</c> after <see cref="LoadCmuDict"/> has completed; otherwise <c>false</c>.</value>
    public bool IsCmuLoaded => _cmuCache.Count > 0;

    /// <summary>
    /// Gets all known poem type definitions.
    /// </summary>
    /// <returns>A read-only collection of <see cref="PoemDefinition"/> entries.</returns>
    public static IReadOnlyCollection<PoemDefinition> GetAllDefinitions() => Definitions.Values.ToList().AsReadOnly();

    /// <summary>
    /// Gets the definition for a specific poem type.
    /// </summary>
    /// <param name="type">The poem type to look up.</param>
    /// <returns>The matching <see cref="PoemDefinition"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a known poem type.</exception>
    public static PoemDefinition GetDefinition(PoemType type) =>
        Definitions.TryGetValue(type, out var def)
            ? def
            : throw new ArgumentException($"Unknown poem type: {type}.", nameof(type));

    /// <summary>
    /// Attempts to get the definition for a specific poem type without throwing.
    /// </summary>
    /// <param name="type">The poem type to look up.</param>
    /// <param name="definition">When returned, contains the definition if found.</param>
    /// <returns><c>true</c> if the type was found; otherwise <c>false</c>.</returns>
    public static bool TryGetDefinition(PoemType type, out PoemDefinition? definition) =>
        Definitions.TryGetValue(type, out definition);

    // =========================================================================
    // CMU Dictionary Loading
    // =========================================================================

    /// <summary>
    /// Loads the CMU pronunciation dictionary from a file into memory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Builds indexes: word-to-phoneme cache and syllable-count-to-words map.
    /// Must be called before <see cref="GeneratePoem"/>.
    /// </para>
    /// </remarks>
    /// <param name="filePath">Path to the CMU dictionary text file.</param>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist at <paramref name="filePath"/>.</exception>
    public void LoadCmuDict(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("CMU dictionary file not found.", filePath);
        }

        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith(";;;"))
            {
                continue;
            }

            // CMU dictionary entries are formatted as "WORD  PHONEME1 PHONEME2 ..."
            // with a double-space separator between the word and its transcription.
            var parts = line.Split("  ", 2, StringSplitOptions.None);
            if (parts.Length < 2)
            {
                continue;
            }

            var rawWord = parts[0];
            // Strip parenthetical disambiguation suffixes (e.g., "WORD(2)") that
            // mark duplicate entries for homographs in the CMU dictionary.
            var word = CmuDictRawWordExtraction().Replace(rawWord, string.Empty).ToLower();

            var phonemes = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // Syllable count = number of phonemes that carry a stress marker
            // (digits 0, 1, or 2 in Arpabet notation).
            var syllableCount = phonemes.Count(p => p.Any(char.IsDigit));

            if (_cmuCache.TryAdd(word, (syllableCount, phonemes)))
            {
                if (!_wordsBySyllableCount.TryGetValue(syllableCount, out var value))
                {
                    value = [];
                    _wordsBySyllableCount[syllableCount] = value;
                }

                if (!value.Contains(word))
                {
                    value.Add(word);
                }
            }
        }
    }

    /// <summary>
    /// Counts the total syllables in a line of text.
    /// </summary>
    /// <param name="line">The line to evaluate.</param>
    /// <returns>The syllable count, or 0 for blank lines.</returns>
    /// <remarks><para>Syllable Counting — two-tier resolution: CMU → vowel-group heuristic</para></remarks>
    public int CountLineSyllables(string line)
    {
        return string.IsNullOrWhiteSpace(line)
            ? 0
            : line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Sum(CountWordSyllables);
    }

    /// <summary>
    /// Counts syllables in a single word, preferring the CMU dictionary and falling back to a vowel-group heuristic.
    /// </summary>
    /// <param name="word">The word to evaluate. Non-letter characters are stripped before lookup.</param>
    /// <returns>The syllable count, or 0 for blank words.</returns>
    public int CountWordSyllables(string word)
    {
        word = new string([.. word.Where(char.IsLetter)]).ToLower();

        if (string.IsNullOrEmpty(word))
        {
            return 0;
        }

        return _cmuCache.TryGetValue(word, out var entry) ? entry.SyllableCount : VowelGroupCount(word);
    }

    // =========================================================================
    // Validation
    // =========================================================================

    /// <summary>
    /// Validates that a set of lines matches a specific poem type's definition.
    /// </summary>
    /// <param name="type">The poem type to validate against.</param>
    /// <param name="lines">The poem lines to validate.</param>
    /// <returns><c>true</c> if the lines conform to the type's constraints.</returns>
    public bool IsValidPoem(PoemType type, params string[] lines)
    {
        var definition = GetDefinition(type);
        return ValidateAgainstDefinition(definition, lines);
    }

    /// <summary>
    /// Validates that a set of lines matches a given poem definition.
    /// </summary>
    /// <param name="definition">The poem definition to validate against.</param>
    /// <param name="lines">The poem lines to validate.</param>
    /// <returns><c>true</c> if the lines conform to the definition's constraints.</returns>
    public bool IsValidPoem(PoemDefinition definition, params string[] lines) => ValidateAgainstDefinition(definition, lines);

    /// <summary>
    /// Detects the poem type for a set of lines by testing against all known definitions.
    /// </summary>
    /// <param name="lines">The poem lines to classify.</param>
    /// <returns>The matching <see cref="PoemType"/> if found; <c>null</c> if no definition matches.</returns>
    public PoemType? DetectPoemType(params string[] lines)
    {
        foreach (var (type, definition) in Definitions)
        {
            if (ValidateAgainstDefinition(definition, lines))
            {
                return type;
            }
        }
        return null;
    }

    // =========================================================================
    // Generation
    // =========================================================================

    /// <summary>
    /// Generates a random poem of the specified type using words from the loaded CMU dictionary.
    /// </summary>
    /// <param name="type">The type of poem to generate.</param>
    /// <param name="seed">Optional random seed for reproducible generation.</param>
    /// <returns>An array of lines forming the generated poem.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the CMU dictionary has not been loaded via <see cref="LoadCmuDict"/>.</exception>
    public string[] GeneratePoem(PoemType type, int? seed = null)
    {
        if (!IsCmuLoaded)
        {
            throw new InvalidOperationException(
                "CMU dictionary must be loaded before generating poems. Call LoadCmuDict() first."
            );
        }

        var definition = GetDefinition(type);
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();

        if (type == PoemType.Freeform)
        {
            return [];
        }

        if (type == PoemType.Isosyllabic)
        {
            return GenerateEqualLinePoem(rng);
        }

        if (type == PoemType.Monoku)
        {
            return [BuildLine(rng.Next(4, 18), rng)];
        }

        if (type == PoemType.Choka)
        {
            return GenerateChokaPoem(rng);
        }

        return [.. definition.SyllablePattern.Select(target => BuildLine(target, rng))];
    }

    // =========================================================================
    // Analysis
    // =========================================================================

    /// <summary>
    /// Analyzes a poem, returning syllable counts per line, per-word breakdown, and detected type.
    /// </summary>
    /// <param name="lines">The poem lines to analyze.</param>
    /// <returns>A <see cref="PoemAnalysis"/> with line-level and word-level breakdowns.</returns>
    public PoemAnalysis Analyze(params string[] lines)
    {
        return new PoemAnalysis
        {
            Lines =
            [
                .. lines.Select(
                    (line, i) =>
                        new LineAnalysis
                        {
                            LineNumber = i + 1,
                            Text = line,
                            TotalSyllables = CountLineSyllables(line),
                            WordBreakdown = AnalyzeLine(line),
                        }
                ),
            ],
            DetectedType = DetectPoemType(lines),
            TotalSyllables = lines.Sum(CountLineSyllables),
        };
    }

    // =========================================================================
    // Rhyme Detection
    // =========================================================================

    /// <summary>
    /// Determines whether two words rhyme using CMU phoneme keys, falling back to a suffix comparison.
    /// </summary>
    /// <param name="word1">The first word.</param>
    /// <param name="word2">The second word.</param>
    /// <returns><c>true</c> if the words share a rhyme key or suffix; <c>false</c> otherwise.</returns>
    public bool WordsRhyme(string word1, string word2)
    {
        if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (_cmuCache.TryGetValue(word1, out var e1) && _cmuCache.TryGetValue(word2, out var e2))
        {
            var k1 = BuildRhymeKey(e1.Phonemes);
            var k2 = BuildRhymeKey(e2.Phonemes);
            return k1 != null && k2 != null && k1 == k2;
        }

        // Fallback: compare the last two characters as a simple suffix heuristic.
        var lower1 = word1.ToLowerInvariant();
        var lower2 = word2.ToLowerInvariant();
        return lower1.Length >= 3 && lower2.Length >= 3 && lower1[^2..] == lower2[^2..];
    }

    /// <summary>
    /// Determines whether two lines end with rhyming words.
    /// </summary>
    /// <param name="line1">The first line.</param>
    /// <param name="line2">The second line.</param>
    /// <returns><c>true</c> if the last word of each line rhymes.</returns>
    public bool LinesRhyme(string line1, string line2)
    {
        var w1 = GetLastWord(line1);
        var w2 = GetLastWord(line2);
        return w1 != null && w2 != null && WordsRhyme(w1, w2);
    }

    // =========================================================================
    // Private: Validation
    // =========================================================================

    private bool ValidateAgainstDefinition(PoemDefinition definition, string[] lines)
    {
        switch (definition.Type)
        {
            case PoemType.Freeform:
                return true;

            case PoemType.Monoku:
                return lines.Length == 1 && CountLineSyllables(lines[0]) is >= 4 and <= 17;

            case PoemType.Isosyllabic:
                return ValidateIsosyllabic(lines);

            case PoemType.Choka:
                return ValidateChoka(lines);

            default:
                if (definition.SyllablePattern.Length > 0 && lines.Length != definition.SyllablePattern.Length)
                {
                    return false;
                }

                for (var i = 0; i < lines.Length; i++)
                {
                    if (definition.SyllablePattern[i] > 0 && CountLineSyllables(lines[i]) != definition.SyllablePattern[i])
                    {
                        return false;
                    }
                }

                return true;
        }
    }

    private static bool ValidateIsosyllabic(string[] lines)
    {
        if (lines.Length < 2)
        {
            return false;
        }

        var firstSyllables = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Sum(w => CountWordSyllablesStatic(w));
        if (firstSyllables == 0)
        {
            return false;
        }

        for (var i = 1; i < lines.Length; i++)
        {
            var currentSyllables = lines[i]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Sum(w => CountWordSyllablesStatic(w));
            if (currentSyllables != firstSyllables)
            {
                return false;
            }
        }

        return true;
    }

    private static int CountWordSyllablesStatic(string word)
    {
        word = new string([.. word.Where(char.IsLetter)]).ToLower();

        if (string.IsNullOrEmpty(word))
        {
            return 0;
        }

        return VowelGroupCount(word);
    }

    private bool ValidateChoka(string[] lines)
    {
        if (lines.Length < 3 || lines.Length % 2 == 0)
        {
            return false;
        }

        for (var i = 0; i < lines.Length - 2; i++)
        {
            var expected = i % 2 == 0 ? 5 : 7;
            if (CountLineSyllables(lines[i]) != expected)
            {
                return false;
            }
        }

        return CountLineSyllables(lines[^2]) == 5 && CountLineSyllables(lines[^1]) == 7;
    }

    // =========================================================================
    // Private: Generation
    // =========================================================================

    private string BuildLine(int targetSyllables, Random rng)
    {
        // Retry up to MaxAttempts times to assemble an exact-syllable line via
        // random word selection. Falls back to a descriptive placeholder when no
        // combination satisfies the target count.
        const int maxAttempts = 2000;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var words = new List<string>();
            var budget = targetSyllables;

            while (budget > 0)
            {
                var candidates = GetCandidateWords(budget, rng);
                if (candidates.Count == 0)
                {
                    break;
                }

                var pick = candidates[rng.Next(candidates.Count)];
                words.Add(pick);
                budget -= CountWordSyllables(pick);
            }

            if (budget == 0)
            {
                return string.Join(" ", words);
            }
        }

        return $"[{targetSyllables} syllables]";
    }

    private string[] GenerateEqualLinePoem(Random rng)
    {
        var lineCount = rng.Next(2, 6);
        var syllables = rng.Next(3, 8);
        var lines = new string[lineCount];

        for (var i = 0; i < lineCount; i++)
        {
            lines[i] = BuildLine(syllables, rng);
        }

        return lines;
    }

    private string[] GenerateChokaPoem(Random rng)
    {
        var lineCount = rng.Next(3, 15);
        if (lineCount % 2 == 0)
        {
            lineCount++;
        }

        var lines = new string[lineCount];
        for (var i = 0; i < lineCount - 2; i++)
        {
            lines[i] = BuildLine(i % 2 == 0 ? 5 : 7, rng);
        }

        lines[^2] = BuildLine(5, rng);
        lines[^1] = BuildLine(7, rng);
        return lines;
    }

    private List<string> GetCandidateWords(int maxSyllables, Random rng)
    {
        var pool = Enumerable
            .Range(1, maxSyllables)
            .Where(_wordsBySyllableCount.ContainsKey)
            .SelectMany(n => _wordsBySyllableCount[n])
            .ToList();

        // Shuffle and cap to 50 candidates to inject randomness while keeping
        // selection cost predictable regardless of dictionary size.
        return [.. pool.OrderBy(_ => rng.Next()).Take(50)];
    }

    // =========================================================================
    // Private: Rhyme / Phoneme helpers
    // =========================================================================

    // Builds a rhyme key from phonemes starting at the last stressed vowel onwards,
    // stripping stress markers so vowels with different stress patterns can still rhyme.
    private static string? BuildRhymeKey(string[] phonemes)
    {
        var lastStress = Array.FindLastIndex(phonemes, p => p.EndsWith('1'));
        if (lastStress < 0)
        {
            lastStress = Array.FindLastIndex(phonemes, p => p.Any(char.IsDigit));
        }

        if (lastStress < 0)
        {
            return null;
        }

        return string.Join(" ", phonemes.Skip(lastStress).Select(p => p.TrimEnd('0', '1', '2')));
    }

    private static string? GetLastWord(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return null;
        }

        return new string([.. words[^1].Where(char.IsLetter)]).ToLowerInvariant();
    }

    // =========================================================================
    // Private: Analysis
    // =========================================================================

    private List<WordAnalysis> AnalyzeLine(string line) =>
        [
            .. line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w =>
                {
                    var clean = new string([.. w.Where(char.IsLetter)]).ToLowerInvariant();
                    var (count, tier) = ResolveWithTier(clean);
                    return new WordAnalysis
                    {
                        Word = w,
                        Syllables = count,
                        Tier = tier,
                    };
                }),
        ];

    private (int Count, string Tier) ResolveWithTier(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return (0, "none");
        }

        if (_cmuCache.TryGetValue(word, out var entry))
        {
            return (entry.SyllableCount, "CMU");
        }

        return (VowelGroupCount(word), "VowelGroup");
    }

    // =========================================================================
    // Private: Vowel-group heuristic (last-resort syllable counter)
    // =========================================================================

    private static int VowelGroupCount(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return 0;
        }

        var count = 0;
        var lastWasVowel = false;

        foreach (var c in word)
        {
            if (Vowels.Contains(c))
            {
                if (!lastWasVowel)
                {
                    count++;
                }

                lastWasVowel = true;
            }
            else
            {
                lastWasVowel = false;
            }
        }

        // Account for silent 'e' at end of word.
        if (word.Length > 2 && word.EndsWith('e') && !Vowels.Contains(word[^2]))
        {
            count--;
        }

        return Math.Max(1, count);
    }

    // =========================================================================
    // Supporting record types
    // =========================================================================

    /// <summary>
    /// Contains the full analysis of a poem, including per-line breakdowns and type detection.
    /// </summary>
    public record PoemAnalysis
    {
        /// <summary>
        /// Gets the per-line analysis results, one entry per line of the poem.
        /// </summary>
        /// <value>A list of <see cref="LineAnalysis"/> entries.</value>
        public List<LineAnalysis> Lines { get; init; } = [];

        /// <summary>
        /// Gets the detected poem type, if any definition matched.
        /// </summary>
        /// <value>The detected type, or <c>null</c> if no definition matched.</value>
        public PoemType? DetectedType { get; init; }

        /// <summary>
        /// Gets the total syllable count across all lines.
        /// </summary>
        /// <value>The sum of all line syllable counts.</value>
        public int TotalSyllables { get; init; }
    }

    /// <summary>
    /// Contains the analysis results for a single line of a poem.
    /// </summary>
    public record LineAnalysis
    {
        /// <summary>
        /// Gets the one-based line number within the poem.
        /// </summary>
        /// <value>The line position, starting at 1.</value>
        public int LineNumber { get; init; }

        /// <summary>
        /// Gets the original text of the line.
        /// </summary>
        /// <value>The line text.</value>
        public string Text { get; init; } = string.Empty;

        /// <summary>
        /// Gets the total syllable count for this line.
        /// </summary>
        /// <value>The syllable count.</value>
        public int TotalSyllables { get; init; }

        /// <summary>
        /// Gets the per-word syllable breakdown for this line.
        /// </summary>
        /// <value>A list of <see cref="WordAnalysis"/> entries.</value>
        public List<WordAnalysis> WordBreakdown { get; init; } = [];
    }

    /// <summary>
    /// Contains the syllable analysis for a single word.
    /// </summary>
    public record WordAnalysis
    {
        /// <summary>
        /// Gets the original word text.
        /// </summary>
        /// <value>The word as it appeared in the poem.</value>
        public string Word { get; init; } = string.Empty;

        /// <summary>
        /// Gets the number of syllables in this word.
        /// </summary>
        /// <value>The syllable count.</value>
        public int Syllables { get; init; }

        /// <summary>
        /// Gets the resolution tier that produced the syllable count.
        /// </summary>
        /// <value>"CMU" if from the pronunciation dictionary, "VowelGroup" if from the fallback heuristic, or "none" if the word was empty.</value>
        public string Tier { get; init; } = string.Empty;
    }

    [GeneratedRegex(@"\(\d+\)$")]
    private static partial Regex CmuDictRawWordExtraction();
}
