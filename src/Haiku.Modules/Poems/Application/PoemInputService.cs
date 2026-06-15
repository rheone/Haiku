using NewSyllableEngine = Haiku.Modules.Poems.Syllables.SyllableEngine;

namespace Haiku.Modules.Poems.Application;

/// <summary>
///     Processes raw poem text input by normalizing content, tokenizing lines,
///     counting syllables, and detecting poem types through the classifier chain.
/// </summary>
internal sealed class PoemInputService : IPoemInputService
{
    private readonly NewSyllableEngine _syllableEngine;
    private readonly PoemClassifierChain _classifierChain;
    private readonly IWordTokenizer _tokenizer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PoemInputService"/> class.
    /// </summary>
    /// <param name="syllableEngine">Engine for counting syllables in words.</param>
    /// <param name="classifierChain">Chain of classifiers for detecting poem types.</param>
    /// <param name="tokenizer">Tokenizes text into individual words.</param>
    public PoemInputService(NewSyllableEngine syllableEngine, PoemClassifierChain classifierChain, IWordTokenizer tokenizer)
    {
        _syllableEngine = syllableEngine;
        _classifierChain = classifierChain;
        _tokenizer = tokenizer;
    }

    /// <summary>
    ///     Processes raw poem input: normalizes text, tokenizes lines, counts syllables,
    ///     and detects the poem type via the classifier chain.
    /// </summary>
    /// <param name="rawContent">Raw poem text from user input.</param>
    /// <returns>
    ///     A <see cref="PoemInputResult"/> containing the processed data, detected type,
    ///     and any validation errors.
    /// </returns>
    public PoemInputResult Process(string rawContent)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(rawContent))
        {
            errors.Add("Poem is empty");
            return new PoemInputResult { Errors = errors.AsReadOnly() };
        }

        var normalized = rawContent.Trim();
        normalized = normalized.Replace("\r\n", "\n").Replace("\r", "\n");
        // Strip zero-width and invisible Unicode codepoints (zero-width space,
        // zero-width non-joiner, directional marks, byte-order mark) that users
        // may inadvertently paste into poem text, which would distort display
        // and throw off syllable counting.
        normalized = new string(
            normalized.Where(c => c != '\u200B' && c != '\u200C' && c != '\u200E' && c != '\u200F' && c != '\uFEFF').ToArray()
        );

        var lines = normalized.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var tokenizedLines = lines.Select(l => _tokenizer.Tokenize(l)).ToArray();
        var lineSyllableCounts = tokenizedLines
            .Select(t => t.Words.Sum(w => _syllableEngine.CountWordSyllables(w).Count))
            .ToArray();
        var totalSyllables = lineSyllableCounts.Sum();

        var poemDefinition = lines.Length > 0 ? _classifierChain.Match(lines, lineSyllableCounts, tokenizedLines) : null;

        var detectedType = poemDefinition?.Type;

        return new PoemInputResult
        {
            NormalizedContent = normalized,
            Lines = lines,
            LineSyllableCounts = lineSyllableCounts,
            TotalSyllables = totalSyllables,
            DetectedType = detectedType,
            PoemDefinition = poemDefinition,
            Errors = errors.AsReadOnly(),
        };
    }
}
