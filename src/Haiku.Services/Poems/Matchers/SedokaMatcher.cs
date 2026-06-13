using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 5-7-7-5-7-7 syllable pattern (Sedoka).</summary>
internal sealed class SedokaMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 11;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 6)
        {
            return null;
        }

        return
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 7
            && syllableCounts[3] == 5
            && syllableCounts[4] == 7
            && syllableCounts[5] == 7
            ? PoemType.Sedoka
            : null;
    }
}
