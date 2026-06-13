using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 5-3-5 syllable pattern (Kelly Lune).</summary>
internal sealed class KellyLuneMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 5;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 3)
        {
            return null;
        }

        return syllableCounts[0] == 5 && syllableCounts[1] == 3 && syllableCounts[2] == 5 ? PoemType.KellyLune : null;
    }
}
