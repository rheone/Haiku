using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 5-7-5 syllable pattern (Haiku).</summary>
internal sealed class HaikuMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 2;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 3)
        {
            return null;
        }

        return syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 5 ? PoemType.Haiku : null;
    }
}
