using Haiku.Domain.Enums;
using Haiku.Services.Haiku;
using Haiku.Services.Poems.Matchers;

namespace Haiku.Services.Poems;

/// <summary>Normalizes raw poem input, counts syllables, detects poem type, and validates content.</summary>
internal sealed class PoemInputService : IPoemInputService
{
    private readonly SyllableEngine _syllableEngine;
    private readonly IPoemMatcherChain _matcherChain;

    /// <summary>Initializes a new instance of the <see cref="PoemInputService"/> class.</summary>
    /// <param name="syllableEngine">The engine used for counting syllables per line.</param>
    /// <param name="matcherChain">The chain of matchers used for poem type detection.</param>
    public PoemInputService(SyllableEngine syllableEngine, IPoemMatcherChain matcherChain)
    {
        _syllableEngine = syllableEngine;
        _matcherChain = matcherChain;
    }

    /// <inheritdoc/>
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
        normalized = new string(
            normalized.Where(c => c != '\u200B' && c != '\u200C' && c != '\u200E' && c != '\u200F' && c != '\uFEFF').ToArray()
        );

        var lines = normalized.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var lineSyllableCounts = lines.Select(l => _syllableEngine.CountLineSyllables(l).Sum()).ToArray();
        var totalSyllables = lineSyllableCounts.Sum();
        PoemType? detectedType = lines.Length > 0 ? _matcherChain.Match(lines, lineSyllableCounts) : null;

        return new PoemInputResult
        {
            NormalizedContent = normalized,
            Lines = lines,
            LineSyllableCounts = lineSyllableCounts,
            TotalSyllables = totalSyllables,
            DetectedType = detectedType,
            Errors = errors.AsReadOnly(),
        };
    }
}
