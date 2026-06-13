using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 4-6-4 syllable pattern (Near-Traditional).</summary>
internal sealed class NearTraditionalMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 7;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 3)
        {
            return null;
        }

        return syllableCounts[0] == 4 && syllableCounts[1] == 6 && syllableCounts[2] == 4 ? PoemType.NearTraditional : null;
    }
}
