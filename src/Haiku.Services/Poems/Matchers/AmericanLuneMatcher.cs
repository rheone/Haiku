using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 3-5-3 syllable pattern (American Lune).</summary>
internal sealed class AmericanLuneMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 4;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 3)
        {
            return null;
        }

        return syllableCounts[0] == 3 && syllableCounts[1] == 5 && syllableCounts[2] == 3 ? PoemType.AmericanLune : null;
    }
}
