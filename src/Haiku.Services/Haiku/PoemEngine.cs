using System.Text.RegularExpressions;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Haiku;

public class PoemEngine
{
    private static readonly IReadOnlyDictionary<PoemType, PoemDefinition> Definitions = new Dictionary<PoemType, PoemDefinition>
    {
        [PoemType.Haiku] = new()
        {
            Type = PoemType.Haiku,
            Name = "Haiku",
            Description = "A traditional Japanese form with 5-7-5 syllable pattern across three lines.",
            SyllablePattern = new[] { 5, 7, 5 }
        },
        [PoemType.Monoku] = new()
        {
            Type = PoemType.Monoku,
            Name = "Monoku",
            Description = "A one-line haiku variant, typically up to 17 syllables.",
            SyllablePattern = new[] { 0 },
            AllowVariableSyllables = true
        },
        [PoemType.Tanka] = new()
        {
            Type = PoemType.Tanka,
            Name = "Tanka",
            Description = "An older Japanese form with 5-7-5-7-7 syllable pattern across five lines.",
            SyllablePattern = new[] { 5, 7, 5, 7, 7 }
        },
        [PoemType.Minimalist] = new()
        {
            Type = PoemType.Minimalist,
            Name = "Minimalist Haiku",
            Description = "A compressed haiku variant with 3-5-3 syllable pattern.",
            SyllablePattern = new[] { 3, 5, 3 }
        },
        [PoemType.Compressed] = new()
        {
            Type = PoemType.Compressed,
            Name = "Compressed Haiku",
            Description = "An ultra-compressed haiku variant with 2-3-2 syllable pattern.",
            SyllablePattern = new[] { 2, 3, 2 }
        },
        [PoemType.NearTraditional] = new()
        {
            Type = PoemType.NearTraditional,
            Name = "Near-Traditional Haiku",
            Description = "A near-traditional haiku variant with 4-6-4 syllable pattern, common in translations.",
            SyllablePattern = new[] { 4, 6, 4 }
        },
        [PoemType.EqualLine] = new()
        {
            Type = PoemType.EqualLine,
            Name = "Equal-Line Poem",
            Description = "A poem where each line has the same syllable count.",
            SyllablePattern = Array.Empty<int>(),
            AllowVariableSyllables = true
        },
        [PoemType.Freeform] = new()
        {
            Type = PoemType.Freeform,
            Name = "Freeform",
            Description = "Poetry without fixed syllable or rhyme constraints.",
            SyllablePattern = Array.Empty<int>(),
            AllowVariableSyllables = true
        },
        [PoemType.Sonnet] = new()
        {
            Type = PoemType.Sonnet,
            Name = "Sonnet",
            Description = "A 14-line poem in iambic pentameter (10 syllables per line) with ABABCDCDEFEFGG rhyme scheme.",
            SyllablePattern = Enumerable.Repeat(10, 14).ToArray(),
            RhymeScheme = "ABABCDCDEFEFGG"
        },
        [PoemType.Limerick] = new()
        {
            Type = PoemType.Limerick,
            Name = "Limerick",
            Description = "A humorous five-line poem with AABBA rhyme scheme and 8-8-5-5-8 syllable pattern.",
            SyllablePattern = new[] { 8, 8, 5, 5, 8 },
            RhymeScheme = "AABBA"
        },
        [PoemType.Cinquain] = new()
        {
            Type = PoemType.Cinquain,
            Name = "Cinquain",
            Description = "A five-line poem with 2-4-6-8-2 syllable pattern.",
            SyllablePattern = new[] { 2, 4, 6, 8, 2 }
        },
        [PoemType.Couplet] = new()
        {
            Type = PoemType.Couplet,
            Name = "Couplet",
            Description = "A two-line poem with AA rhyme scheme. Syllable count per line is flexible.",
            SyllablePattern = new[] { 0, 0 },
            RhymeScheme = "AA",
            AllowVariableSyllables = true
        },
        [PoemType.Quatrain] = new()
        {
            Type = PoemType.Quatrain,
            Name = "Quatrain",
            Description = "A four-line poem with AABB rhyme scheme. Syllable count per line is flexible.",
            SyllablePattern = new[] { 0, 0, 0, 0 },
            RhymeScheme = "AABB",
            AllowVariableSyllables = true
        }
    };

    private readonly Dictionary<string, (int SyllableCount, string[] Phonemes)> _cmuCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, List<string>> _wordsBySyllableCount = new();
    private readonly Dictionary<string, List<string>> _wordsByRhymeKey = new();
    private const string Vowels = "aeiouy";

    public bool IsCmuLoaded => _cmuCache.Count > 0;

    public static IReadOnlyCollection<PoemDefinition> GetAllDefinitions() => Definitions.Values.ToList().AsReadOnly();

    public static PoemDefinition GetDefinition(PoemType type) =>
        Definitions.TryGetValue(type, out var def)
            ? def
            : throw new ArgumentException($"Unknown poem type: {type}.", nameof(type));

    public static bool TryGetDefinition(PoemType type, out PoemDefinition? definition) =>
        Definitions.TryGetValue(type, out definition);

    // =========================================================================
    // CMU Dictionary Loading
    // =========================================================================

    public void LoadCmuDict(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("CMU dictionary file not found.", filePath);

        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith(";;;")) continue;

            var parts = line.Split("  ", 2, StringSplitOptions.None);
            if (parts.Length < 2) continue;

            var rawWord = parts[0];
            var word = Regex.Replace(rawWord, @"\(\d+\)$", "").ToLower();

            var phonemes = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var syllableCount = phonemes.Count(p => p.Any(char.IsDigit));

            if (_cmuCache.TryAdd(word, (syllableCount, phonemes)))
            {
                if (!_wordsBySyllableCount.ContainsKey(syllableCount))
                    _wordsBySyllableCount[syllableCount] = new List<string>();
                if (!_wordsBySyllableCount[syllableCount].Contains(word))
                    _wordsBySyllableCount[syllableCount].Add(word);

                var rhymeKey = BuildRhymeKey(phonemes);
                if (rhymeKey != null)
                {
                    if (!_wordsByRhymeKey.ContainsKey(rhymeKey))
                        _wordsByRhymeKey[rhymeKey] = new List<string>();
                    if (!_wordsByRhymeKey[rhymeKey].Contains(word))
                        _wordsByRhymeKey[rhymeKey].Add(word);
                }
            }
        }
    }

    // =========================================================================
    // Syllable Counting — two-tier resolution: CMU → vowel-group heuristic
    // =========================================================================

    public int CountLineSyllables(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return 0;
        return line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Sum(CountWordSyllables);
    }

    public int CountWordSyllables(string word)
    {
        word = new string(word.Where(char.IsLetter).ToArray()).ToLower();
        if (string.IsNullOrEmpty(word)) return 0;

        if (_cmuCache.TryGetValue(word, out var entry))
            return entry.SyllableCount;

        return VowelGroupCount(word);
    }

    // =========================================================================
    // Validation
    // =========================================================================

    public bool IsValidPoem(PoemType type, params string[] lines)
    {
        var definition = GetDefinition(type);
        return ValidateAgainstDefinition(definition, lines);
    }

    public bool IsValidPoem(PoemDefinition definition, params string[] lines) =>
        ValidateAgainstDefinition(definition, lines);

    public PoemType? DetectPoemType(params string[] lines)
    {
        foreach (var (type, definition) in Definitions)
        {
            if (ValidateAgainstDefinition(definition, lines))
                return type;
        }
        return null;
    }

    // =========================================================================
    // Generation
    // =========================================================================

    public string[] GeneratePoem(PoemType type, int? seed = null)
    {
        if (!IsCmuLoaded)
            throw new InvalidOperationException("CMU dictionary must be loaded before generating poems. Call LoadCmuDict() first.");

        var definition = GetDefinition(type);
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();

        if (definition.AllowVariableSyllables || definition.SyllablePattern.Length == 0)
        {
            if (definition.Type == PoemType.EqualLine)
                return GenerateEqualLinePoem(rng);

            if (definition.SyllablePattern.Length == 0 && definition.Type == PoemType.Freeform)
                return Array.Empty<string>();

            return definition.SyllablePattern.Select(target =>
                target > 0 ? BuildLine(target, rng) : BuildLine(rng.Next(5, 11), rng)).ToArray();
        }

        return definition.RhymeScheme != null
            ? GenerateRhymingPoem(definition, rng)
            : definition.SyllablePattern.Select(target => BuildLine(target, rng)).ToArray();
    }

    // =========================================================================
    // Analysis
    // =========================================================================

    public PoemAnalysis Analyse(params string[] lines)
    {
        return new PoemAnalysis
        {
            Lines = lines.Select((line, i) => new LineAnalysis
            {
                LineNumber = i + 1,
                Text = line,
                TotalSyllables = CountLineSyllables(line),
                WordBreakdown = AnalyseLine(line)
            }).ToList(),
            DetectedType = DetectPoemType(lines),
            TotalSyllables = lines.Sum(CountLineSyllables)
        };
    }

    // =========================================================================
    // Rhyme Detection
    // =========================================================================

    public bool WordsRhyme(string word1, string word2)
    {
        if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
            return true;

        if (_cmuCache.TryGetValue(word1, out var e1) && _cmuCache.TryGetValue(word2, out var e2))
        {
            var k1 = BuildRhymeKey(e1.Phonemes);
            var k2 = BuildRhymeKey(e2.Phonemes);
            return k1 != null && k2 != null && k1 == k2;
        }

        var lower1 = word1.ToLowerInvariant();
        var lower2 = word2.ToLowerInvariant();
        return lower1.Length >= 3 && lower2.Length >= 3 && lower1[^2..] == lower2[^2..];
    }

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
        if (definition.SyllablePattern.Length > 0 && lines.Length != definition.SyllablePattern.Length)
            return false;

        if (!definition.AllowVariableSyllables && definition.SyllablePattern.Length > 0)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (definition.SyllablePattern[i] > 0 && CountLineSyllables(lines[i]) != definition.SyllablePattern[i])
                    return false;
            }
        }

        if (definition.Type == PoemType.EqualLine && lines.Length > 0)
        {
            var firstSyllables = CountLineSyllables(lines[0]);
            if (firstSyllables == 0) return false;
            for (int i = 1; i < lines.Length; i++)
            {
                if (CountLineSyllables(lines[i]) != firstSyllables)
                    return false;
            }
        }

        if (definition.Type == PoemType.Monoku && lines.Length == 1)
        {
            var total = CountLineSyllables(lines[0]);
            if (total < 1 || total > 17) return false;
        }

        if (definition.RhymeScheme != null && lines.Length >= 2)
        {
            if (!ValidateRhymeScheme(lines, definition.RhymeScheme))
                return false;
        }

        return true;
    }

    private bool ValidateRhymeScheme(string[] lines, string scheme)
    {
        var rhymeGroups = new Dictionary<char, string>();

        for (int i = 0; i < Math.Min(lines.Length, scheme.Length); i++)
        {
            var lastWord = GetLastWord(lines[i]);
            if (lastWord == null) continue;

            if (rhymeGroups.TryGetValue(scheme[i], out var anchor))
            {
                if (!WordsRhyme(lastWord, anchor))
                    return false;
            }
            else
            {
                rhymeGroups[scheme[i]] = lastWord;
            }
        }

        return true;
    }

    // =========================================================================
    // Private: Generation
    // =========================================================================

    private string BuildLine(int targetSyllables, Random rng)
    {
        const int maxAttempts = 2000;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var words = new List<string>();
            int budget = targetSyllables;

            while (budget > 0)
            {
                var candidates = GetCandidateWords(budget, rng);
                if (candidates.Count == 0) break;

                var pick = candidates[rng.Next(candidates.Count)];
                words.Add(pick);
                budget -= CountWordSyllables(pick);
            }

            if (budget == 0)
                return string.Join(" ", words);
        }

        return $"[{targetSyllables} syllables]";
    }

    private string[] GenerateRhymingPoem(PoemDefinition definition, Random rng)
    {
        var scheme = definition.RhymeScheme!;
        var lines = new string[definition.SyllablePattern.Length];
        var lineLastWords = new Dictionary<int, string>();

        var rhymeGroups = new Dictionary<char, List<int>>();
        for (int i = 0; i < Math.Min(lines.Length, scheme.Length); i++)
        {
            var letter = scheme[i];
            if (!rhymeGroups.ContainsKey(letter))
                rhymeGroups[letter] = new List<int>();
            rhymeGroups[letter].Add(i);
        }

        foreach (var group in rhymeGroups.Values)
        {
            if (group.Count < 2)
            {
                foreach (var idx in group)
                    lineLastWords[idx] = PickAnyWord(rng);
                continue;
            }

            var viableKeys = _wordsByRhymeKey
                .Where(kvp => kvp.Value.Count >= group.Count)
                .Select(kvp => kvp.Key)
                .ToList();

            if (viableKeys.Count == 0)
            {
                foreach (var idx in group)
                    lineLastWords[idx] = PickAnyWord(rng);
                continue;
            }

            var chosenKey = viableKeys[rng.Next(viableKeys.Count)];
            var candidates = _wordsByRhymeKey[chosenKey]
                .OrderBy(_ => rng.Next())
                .Take(group.Count)
                .ToList();

            for (int j = 0; j < group.Count; j++)
                lineLastWords[group[j]] = candidates[j];
        }

        for (int i = 0; i < lines.Length; i++)
        {
            var target = definition.SyllablePattern[i];
            lines[i] = lineLastWords.TryGetValue(i, out var lastWord)
                ? BuildLineWithLastWord(target, lastWord, rng)
                : BuildLine(target, rng);
        }

        return lines;
    }

    private string BuildLineWithLastWord(int targetSyllables, string lastWord, Random rng)
    {
        const int maxAttempts = 2000;
        int lastWordSyllables = CountWordSyllables(lastWord);
        int remainingBudget = targetSyllables - lastWordSyllables;

        if (remainingBudget == 0) return lastWord;
        if (remainingBudget < 0) return $"[{targetSyllables} syllables]";

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var words = new List<string>();
            int budget = remainingBudget;

            while (budget > 0)
            {
                var candidates = GetCandidateWords(budget, rng);
                if (candidates.Count == 0) break;

                var pick = candidates[rng.Next(candidates.Count)];
                words.Add(pick);
                budget -= CountWordSyllables(pick);
            }

            if (budget == 0)
            {
                words.Add(lastWord);
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

        for (int i = 0; i < lineCount; i++)
            lines[i] = BuildLine(syllables, rng);

        return lines;
    }

    private string PickAnyWord(Random rng)
    {
        if (_wordsBySyllableCount.Count == 0) return "word";
        var keys = _wordsBySyllableCount.Keys.ToList();
        var bucket = _wordsBySyllableCount[keys[rng.Next(keys.Count)]];
        return bucket[rng.Next(bucket.Count)];
    }

    private List<string> GetCandidateWords(int maxSyllables, Random rng)
    {
        var pool = Enumerable.Range(1, maxSyllables)
            .Where(_wordsBySyllableCount.ContainsKey)
            .SelectMany(n => _wordsBySyllableCount[n])
            .ToList();

        return pool.OrderBy(_ => rng.Next()).Take(50).ToList();
    }

    // =========================================================================
    // Private: Rhyme / Phoneme helpers
    // =========================================================================

    private static string? BuildRhymeKey(string[] phonemes)
    {
        int lastStress = Array.FindLastIndex(phonemes, p => p.EndsWith('1'));
        if (lastStress < 0)
            lastStress = Array.FindLastIndex(phonemes, p => p.Any(char.IsDigit));

        if (lastStress < 0) return null;

        return string.Join(" ", phonemes
            .Skip(lastStress)
            .Select(p => p.TrimEnd('0', '1', '2')));
    }

    private static string? GetLastWord(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;
        var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return null;
        return new string(words[^1].Where(char.IsLetter).ToArray()).ToLowerInvariant();
    }

    // =========================================================================
    // Private: Analysis
    // =========================================================================

    private List<WordAnalysis> AnalyseLine(string line) =>
        line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w =>
            {
                var clean = new string(w.Where(char.IsLetter).ToArray()).ToLowerInvariant();
                var (count, tier) = ResolveWithTier(clean);
                return new WordAnalysis { Word = w, Syllables = count, Tier = tier };
            })
            .ToList();

    private (int Count, string Tier) ResolveWithTier(string word)
    {
        if (string.IsNullOrEmpty(word)) return (0, "none");

        if (_cmuCache.TryGetValue(word, out var entry))
            return (entry.SyllableCount, "CMU");

        return (VowelGroupCount(word), "VowelGroup");
    }

    // =========================================================================
    // Private: Vowel-group heuristic (last-resort syllable counter)
    // =========================================================================

    private static int VowelGroupCount(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return 0;

        int count = 0;
        bool lastWasVowel = false;

        foreach (char c in word)
        {
            if (Vowels.Contains(c))
            {
                if (!lastWasVowel) count++;
                lastWasVowel = true;
            }
            else
            {
                lastWasVowel = false;
            }
        }

        if (word.Length > 2 && word.EndsWith('e') && !Vowels.Contains(word[^2]))
            count--;

        return Math.Max(1, count);
    }

    // =========================================================================
    // Supporting record types
    // =========================================================================

    public record PoemAnalysis
    {
        public List<LineAnalysis> Lines { get; init; } = new();
        public PoemType? DetectedType { get; init; }
        public int TotalSyllables { get; init; }
    }

    public record LineAnalysis
    {
        public int LineNumber { get; init; }
        public string Text { get; init; } = string.Empty;
        public int TotalSyllables { get; init; }
        public List<WordAnalysis> WordBreakdown { get; init; } = new();
    }

    public record WordAnalysis
    {
        public string Word { get; init; } = string.Empty;
        public int Syllables { get; init; }
        public string Tier { get; init; } = string.Empty;
    }
}
