using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Evaluates a chain of <see cref="IPoemMatcher"/> instances to detect the poem type.</summary>
public interface IPoemMatcherChain
{
    /// <summary>Matches the given lines and syllable counts to a poem type by iterating through registered matchers in priority order.</summary>
    /// <param name="lines">The individual lines of the poem.</param>
    /// <param name="syllableCounts">The syllable count for each line.</param>
    /// <returns>The detected <see cref="PoemType"/>, or <see cref="PoemType.Freeform"/> if no matcher recognizes the pattern.</returns>
    PoemType Match(string[] lines, int[] syllableCounts);
}
