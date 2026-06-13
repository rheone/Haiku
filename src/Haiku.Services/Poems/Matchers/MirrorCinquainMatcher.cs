using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 2-4-6-8-2-2-8-6-4-2 syllable pattern (Mirror Cinquain).</summary>
internal sealed class MirrorCinquainMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 13;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 10)
        {
            return null;
        }

        return
            syllableCounts[0] == 2
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
            && syllableCounts[4] == 2
            && syllableCounts[5] == 2
            && syllableCounts[6] == 8
            && syllableCounts[7] == 6
            && syllableCounts[8] == 4
            && syllableCounts[9] == 2
            ? PoemType.MirrorCinquain
            : null;
    }
}
