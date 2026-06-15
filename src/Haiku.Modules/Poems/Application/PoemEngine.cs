namespace Haiku.Modules.Poems.Application;

/// <summary>
///     Core poetry engine: generates poems, analyzes structure, and delegates
///     syllable counting, rhyme detection, and type classification to dedicated services.
/// </summary>
/// <remarks>
///     <para>
///         <b>Architecture:</b> This class is a thin orchestrator that delegates:
///         <list type="bullet">
///             <item>Syllable counting → <see cref="SyllableEngine"/></item>
///             <item>Rhyme detection → <see cref="RhymingEngine"/></item>
///             <item>Poem type metadata → <see cref="PoemClassifierChain"/></item>
///             <item>Word lookups for generation → <see cref="CmuDictionaryProvider"/></item>
///         </list>
///     </para>
/// </remarks>
public sealed class PoemEngine
{
    private readonly PoemClassifierChain? _chain;
    private readonly SyllableEngine? _syllableEngine;
    private readonly CmuDictionaryProvider? _cmuDictionary;
    private readonly RhymingEngine? _rhymingEngine;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PoemEngine"/> class.
    /// </summary>
    /// <param name="chain">Optional classifier chain for type metadata and detection.</param>
    /// <param name="syllableEngine">Optional syllable engine for counting.</param>
    /// <param name="cmuDictionary">Optional CMU dictionary for word lookups in poem generation.</param>
    /// <param name="rhymingEngine">Optional rhyming engine for rhyme detection.</param>
    public PoemEngine(
        PoemClassifierChain? chain = null,
        SyllableEngine? syllableEngine = null,
        CmuDictionaryProvider? cmuDictionary = null,
        RhymingEngine? rhymingEngine = null
    )
    {
        _chain = chain;
        _syllableEngine = syllableEngine;
        _cmuDictionary = cmuDictionary;
        _rhymingEngine = rhymingEngine;
    }

    // =========================================================================
    // Metadata
    // =========================================================================

    /// <summary>
    ///     Gets definitions for all registered poem types from the classifier chain.
    /// </summary>
    /// <returns>A read-only collection of <see cref="PoemDefinition"/> entries.</returns>
    public IReadOnlyCollection<PoemDefinition> GetAllDefinitions()
    {
        if (_chain is null)
        {
            return [];
        }

        return _chain.AllTypes.Select(c => ClassifierBuilder.Build(c)).ToList().AsReadOnly();
    }

    /// <summary>
    ///     Gets the definition for a specific poem type from the classifier chain.
    /// </summary>
    /// <param name="type">The poem type to look up.</param>
    /// <returns>The matching <see cref="PoemDefinition"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a known poem type.</exception>
    public PoemDefinition GetDefinition(PoemType type)
    {
        var classifier = _chain?.GetType(type);
        if (classifier is null)
        {
            throw new ArgumentException($"Unknown poem type: {type}.", nameof(type));
        }

        return ClassifierBuilder.Build(classifier);
    }

    /// <summary>
    ///     Attempts to get the definition for a specific poem type without throwing.
    /// </summary>
    /// <param name="type">The poem type to look up.</param>
    /// <param name="definition">When returned, contains the definition if found.</param>
    /// <returns><c>true</c> if the type was found; otherwise <c>false</c>.</returns>
    public bool TryGetDefinition(PoemType type, out PoemDefinition? definition)
    {
        var classifier = _chain?.GetType(type);
        if (classifier is null)
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(classifier);
        return true;
    }

    // =========================================================================
    // Syllable Counting (delegated)
    // =========================================================================

    /// <summary>
    ///     Counts the total syllables in a line of text by delegating to <see cref="SyllableEngine"/>
    ///     when available, falling back to a simple heuristic.
    /// </summary>
    /// <param name="line">The line to evaluate.</param>
    /// <returns>The syllable count, or 0 for blank lines.</returns>
    public int CountLineSyllables(string line)
    {
        if (_syllableEngine is not null)
        {
            return _syllableEngine.CountLineSyllables(line).TotalSyllables;
        }

        return string.IsNullOrWhiteSpace(line)
            ? 0
            : line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Sum(CountWordSyllables);
    }

    /// <summary>
    ///     Counts syllables in a single word by delegating to <see cref="SyllableEngine"/>
    ///     when available, falling back to a simple heuristic.
    /// </summary>
    /// <param name="word">The word to evaluate.</param>
    /// <returns>The syllable count, or 0 for blank words.</returns>
    public int CountWordSyllables(string word)
    {
        if (_syllableEngine is not null)
        {
            return _syllableEngine.CountWordSyllables(word).Count;
        }

        word = new string([.. word.Where(char.IsLetter)]).ToLowerInvariant();
        return string.IsNullOrEmpty(word) ? 0 : Math.Max(1, VowelGroupCountFallback(word));
    }

    // =========================================================================
    // Validation (delegated to classifier chain)
    // =========================================================================

    /// <summary>
    ///     Validates that a set of lines matches a specific poem type by running
    ///     the classifier chain's classifier for that type.
    /// </summary>
    /// <param name="type">The poem type to validate against.</param>
    /// <param name="lines">The poem lines to validate.</param>
    /// <returns><c>true</c> if the lines conform to the type's constraints.</returns>
    public bool IsValidPoem(PoemType type, params string[] lines)
    {
        if (_chain is null)
        {
            return false;
        }

        var classifier = _chain.GetType(type);
        if (classifier is null)
        {
            return false;
        }

        var counts = lines.Select(CountLineSyllables).ToArray();
        var tokenized = lines
            .Select(line => new TokenizedLine
            {
                Words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                WordCount = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            })
            .ToArray();

        return classifier.TryClassify(lines, counts, tokenized, out _);
    }

    /// <summary>
    ///     Validates that a set of lines matches a given poem definition by delegating
    ///     to <see cref="IsValidPoem(PoemType,string[])"/> with the definition's type.
    /// </summary>
    /// <param name="definition">The poem definition to validate against.</param>
    /// <param name="lines">The poem lines to validate.</param>
    /// <returns><c>true</c> if the lines conform to the definition's type constraints.</returns>
    public bool IsValidPoem(PoemDefinition definition, params string[] lines) => IsValidPoem(definition.Type, lines);

    // =========================================================================
    // Detection (delegated to classifier chain)
    // =========================================================================

    /// <summary>
    ///     Detects the poem type for a set of lines using the classifier chain.
    /// </summary>
    /// <param name="lines">The poem lines to classify.</param>
    /// <returns>The matching <see cref="PoemType"/> if found; <c>null</c> if no definition matches.</returns>
    public PoemType? DetectPoemType(params string[] lines)
    {
        if (_chain is null || lines.Length == 0)
        {
            return null;
        }

        var syllableCounts = lines.Select(CountLineSyllables).ToArray();
        var tokenizedLines = lines
            .Select(line => new TokenizedLine
            {
                Words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                WordCount = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            })
            .ToArray();

        var result = _chain.Match(lines, syllableCounts, tokenizedLines);
        return result.Type;
    }

    // =========================================================================
    // Generation (uses CmuDictionaryProvider for word lookups)
    // =========================================================================

    /// <summary>
    ///     Generates a random poem of the specified type using words from the CMU dictionary.
    /// </summary>
    /// <param name="type">The type of poem to generate.</param>
    /// <param name="seed">Optional random seed for reproducible generation.</param>
    /// <returns>An array of lines forming the generated poem.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the CMU dictionary has not been configured.
    /// </exception>
    public string[] GeneratePoem(PoemType type, int? seed = null)
    {
        if (_cmuDictionary is null)
        {
            throw new InvalidOperationException(
                "CMU dictionary must be configured for poem generation. Register CmuDictionaryProvider in DI."
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

        if (definition.SyllablePattern is not null)
        {
            return [.. definition.SyllablePattern.Select(target => BuildLine(target, rng))];
        }

        var pattern = PatternGenerator.GeneratePattern(type, rng);
        if (pattern.Length == 0)
        {
            return [];
        }

        return definition.Scaffold switch
        {
            PoemScaffold.WordBased => [.. pattern.Select(target => BuildLineByWordCount(target, rng))],
            _ => [.. pattern.Select(target => BuildLine(target, rng))],
        };
    }

    // =========================================================================
    // Analysis (uses SyllableEngine for counting)
    // =========================================================================

    /// <summary>
    ///     Analyzes a poem, returning syllable counts per line, per-word breakdown, and detected type.
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
                    {
                        var lineResult = _syllableEngine?.CountLineSyllables(line, i + 1);

                        return new LineAnalysis
                        {
                            LineNumber = i + 1,
                            Text = line,
                            TotalSyllables = lineResult?.TotalSyllables ?? CountLineSyllables(line),
                            WordBreakdown =
                                lineResult
                                    ?.Words.Select(w => new WordAnalysis
                                    {
                                        Word = w.Word,
                                        Syllables = w.Count,
                                        Tier = w.Tier,
                                    })
                                    .ToList()
                                ?? AnalyzeLineFallback(line),
                        };
                    }
                ),
            ],
            DetectedType = DetectPoemType(lines),
            TotalSyllables = lines.Sum(l => _syllableEngine?.CountLineSyllables(l).TotalSyllables ?? CountLineSyllables(l)),
        };
    }

    // =========================================================================
    // Rhyme Detection (delegated to RhymingEngine)
    // =========================================================================

    /// <summary>
    ///     Determines whether two words rhyme, delegating to the injected
    ///     <see cref="RhymingEngine"/> when available. Falls back to a simple
    ///     last-two-character suffix comparison for words of at least three characters.
    /// </summary>
    /// <param name="word1">The first word to compare.</param>
    /// <param name="word2">The second word to compare.</param>
    /// <returns><c>true</c> if the words rhyme; otherwise <c>false</c>.</returns>
    public bool WordsRhyme(string word1, string word2)
    {
        if (_rhymingEngine is not null)
        {
            return _rhymingEngine.WordsRhyme(word1, word2);
        }

        // Fallback: same word or last-2-chars suffix match
        if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var lower1 = word1.ToLowerInvariant();
        var lower2 = word2.ToLowerInvariant();
        return lower1.Length >= 3 && lower2.Length >= 3 && lower1[^2..] == lower2[^2..];
    }

    /// <summary>
    ///     Determines whether two lines end with rhyming words, delegating to the
    ///     injected <see cref="RhymingEngine"/> when available. Falls back to extracting
    ///     the last word of each line and calling <see cref="WordsRhyme"/>.
    /// </summary>
    /// <param name="line1">The first line to compare.</param>
    /// <param name="line2">The second line to compare.</param>
    /// <returns><c>true</c> if the final words of each line rhyme; otherwise <c>false</c>.</returns>
    public bool LinesRhyme(string line1, string line2)
    {
        if (_rhymingEngine is not null)
        {
            return _rhymingEngine.LinesRhyme(line1, line2);
        }

        var w1 = GetLastWordFallback(line1);
        var w2 = GetLastWordFallback(line2);
        return w1 != null && w2 != null && WordsRhyme(w1, w2);
    }

    // =========================================================================
    // Private: Generation helpers
    // =========================================================================

    private string BuildLine(int targetSyllables, Random rng)
    {
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
        if (_cmuDictionary is null)
        {
            return [];
        }

        var pool = new List<string>();
        for (var s = 1; s <= maxSyllables; s++)
        {
            if (_cmuDictionary.TryGetWordsBySyllableCount(s, out var words))
            {
                pool.AddRange(words);
            }
        }

        // Shuffle and cap to 50 candidates to keep selection cost predictable
        return [.. pool.OrderBy(_ => rng.Next()).Take(50)];
    }

    private string BuildLineByWordCount(int targetWordCount, Random rng)
    {
        const int maxAttempts = 2000;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (_cmuDictionary is not null && _cmuDictionary.TryGetWords(targetWordCount, out var words, rng))
            {
                return string.Join(" ", words);
            }
        }

        return $"[{targetWordCount} words]";
    }

    // =========================================================================
    // Private: Fallback helpers (used only when injected services are null)
    // =========================================================================

    private static int VowelGroupCountFallback(string word)
    {
        const string vowels = "aeiouy";
        var count = 0;
        var lastWasVowel = false;

        foreach (var c in word)
        {
            if (vowels.Contains(c))
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

        if (word.Length > 2 && word.EndsWith('e') && !vowels.Contains(word[^2]))
        {
            count--;
        }

        return count;
    }

    private static List<WordAnalysis> AnalyzeLineFallback(string line) =>
        [
            .. line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w =>
                {
                    var clean = new string([.. w.Where(char.IsLetter)]).ToLowerInvariant();
                    return new WordAnalysis
                    {
                        Word = w,
                        Syllables = string.IsNullOrEmpty(clean) ? 1 : Math.Max(1, VowelGroupCountFallback(clean)),
                        Tier = "fallback",
                    };
                }),
        ];

    private static string? GetLastWordFallback(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length > 0 ? new string([.. words[^1].Where(char.IsLetter)]).ToLowerInvariant() : null;
    }

    // =========================================================================
    // Supporting record types
    // =========================================================================

    /// <summary>
    ///     Contains the full analysis of a poem, including per-line breakdowns and type detection.
    /// </summary>
    public record PoemAnalysis
    {
        /// <summary>
        ///     Gets the per-line analysis results, one entry per line of the poem.
        /// </summary>
        /// <value>The list of line-level analysis entries.</value>
        public List<LineAnalysis> Lines { get; init; } = [];

        /// <summary>
        ///     Gets the detected poem type, if any definition matched.
        /// </summary>
        /// <value>The detected <see cref="PoemType"/>, or <c>null</c> if no classifier matched.</value>
        public PoemType? DetectedType { get; init; }

        /// <summary>
        ///     Gets the total syllable count across all lines.
        /// </summary>
        /// <value>The sum of syllable counts for every line in the poem.</value>
        public int TotalSyllables { get; init; }
    }

    /// <summary>
    ///     Contains the analysis results for a single line of a poem.
    /// </summary>
    public record LineAnalysis
    {
        /// <summary>
        ///     Gets the one-based line number within the poem.
        /// </summary>
        /// <value>The position of this line, starting from 1.</value>
        public int LineNumber { get; init; }

        /// <summary>
        ///     Gets the original text of the line.
        /// </summary>
        /// <value>The line content as submitted.</value>
        public string Text { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the total syllable count for this line.
        /// </summary>
        /// <value>The sum of word syllable counts in this line.</value>
        public int TotalSyllables { get; init; }

        /// <summary>
        ///     Gets the per-word syllable breakdown for this line.
        /// </summary>
        /// <value>The list of word-level analysis entries.</value>
        public List<WordAnalysis> WordBreakdown { get; init; } = [];
    }

    /// <summary>
    ///     Contains the syllable analysis for a single word.
    /// </summary>
    public record WordAnalysis
    {
        /// <summary>
        ///     Gets the original word text.
        /// </summary>
        /// <value>The word as it appeared in the input.</value>
        public string Word { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the number of syllables in this word.
        /// </summary>
        /// <value>The syllable count.</value>
        public int Syllables { get; init; }

        /// <summary>
        ///     Gets the resolution tier that produced the syllable count.
        /// </summary>
        /// <value>
        ///     The name of the provider or algorithm that determined the count
        ///     (e.g., "CMU", "Heuristic", "Numeral", "LetterName").
        /// </value>
        public string Tier { get; init; } = string.Empty;
    }
}
