using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 2-4-6-8-2 syllable pattern (American Cinquain).</summary>
internal sealed class AmericanCinquainMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 9;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 5)
        {
            return null;
        }

        return
            syllableCounts[0] == 2
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
            && syllableCounts[4] == 2
            ? PoemType.AmericanCinquain
            : null;
    }
}
