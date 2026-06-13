using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 5-7-7 syllable pattern (Katauta).</summary>
internal sealed class KatautaMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 3;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 3)
        {
            return null;
        }

        return syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 7 ? PoemType.Katauta : null;
    }
}
