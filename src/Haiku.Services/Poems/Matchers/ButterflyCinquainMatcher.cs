using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 2-4-6-8-2-8-6-4-2 syllable pattern (Butterfly Cinquain).</summary>
internal sealed class ButterflyCinquainMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 12;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 9)
        {
            return null;
        }

        return
            syllableCounts[0] == 2
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
            && syllableCounts[4] == 2
            && syllableCounts[5] == 8
            && syllableCounts[6] == 6
            && syllableCounts[7] == 4
            && syllableCounts[8] == 2
            ? PoemType.ButterflyCinquain
            : null;
    }
}
