using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Attempts to match a set of lines and syllable counts to a specific poem type.</summary>
public interface IPoemMatcher
{
    /// <summary>Gets the priority order for this matcher.</summary>
    /// <value>Lower values are evaluated first.</value>
    int Priority { get; }

    /// <summary>Attempts to match the given lines and syllable counts to a poem type.</summary>
    /// <param name="lines">The individual lines of the poem.</param>
    /// <param name="syllableCounts">The syllable count for each line.</param>
    /// <returns>The matching <see cref="PoemType"/> if recognized; otherwise <c>null</c>.</returns>
    PoemType? TryMatch(string[] lines, int[] syllableCounts);
}
