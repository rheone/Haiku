using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 5-7-5-7-7 syllable pattern (Tanka).</summary>
internal sealed class TankaMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 8;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 5)
        {
            return null;
        }

        return
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 5
            && syllableCounts[3] == 7
            && syllableCounts[4] == 7
            ? PoemType.Tanka
            : null;
    }
}
