using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 2-8-6-4-2 syllable pattern (Reverse Cinquain).</summary>
internal sealed class ReverseCinquainMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 10;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 5)
        {
            return null;
        }

        return
            syllableCounts[0] == 2
            && syllableCounts[1] == 8
            && syllableCounts[2] == 6
            && syllableCounts[3] == 4
            && syllableCounts[4] == 2
            ? PoemType.ReverseCinquain
            : null;
    }
}
