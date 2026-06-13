using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches single-line poems with 4-17 syllables (Monoku).</summary>
internal sealed class MonokuMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 1;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 1)
        {
            return null;
        }

        var syllables = syllableCounts[0];
        return syllables >= 4 && syllables <= 17 ? PoemType.Monoku : null;
    }
}
