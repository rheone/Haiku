using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Matches poems where every line has the same syllable count (Isosyllabic).</summary>
internal sealed class IsosyllabicMatcher : IPoemMatcher
{
    /// <inheritdoc/>
    public int Priority => 15;

    /// <inheritdoc/>
    public PoemType? TryMatch(string[] lines, int[] syllableCounts)
    {
        if (lines.Length < 2)
        {
            return null;
        }

        var first = syllableCounts[0];

        for (var i = 1; i < syllableCounts.Length; i++)
        {
            if (syllableCounts[i] != first)
            {
                return null;
            }
        }

        return PoemType.Isosyllabic;
    }
}
