using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a repeating 5-7 pattern ending in 5-7-7 (Choka).</summary>
internal sealed class ChokaMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 14;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        var lineCount = lines.Length;

        if (lineCount < 7 || lineCount % 2 == 0)
        {
            return null;
        }

        var alternatingEnd = lineCount - 3;

        for (var i = 0; i < alternatingEnd; i++)
        {
            var expected = i % 2 == 0 ? 5 : 7;
            if (syllableCounts[i] != expected)
            {
                return null;
            }
        }

        return syllableCounts[^3] == 5 && syllableCounts[^2] == 7 && syllableCounts[^1] == 7 ? PoemType.Choka : null;
    }
}
