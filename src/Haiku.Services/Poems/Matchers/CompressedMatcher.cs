using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems with a 2-3-2 syllable pattern (Compressed).</summary>
internal sealed class CompressedMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 6;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length != 3)
        {
            return null;
        }

        return syllableCounts[0] == 2 && syllableCounts[1] == 3 && syllableCounts[2] == 2 ? PoemType.Compressed : null;
    }
}
