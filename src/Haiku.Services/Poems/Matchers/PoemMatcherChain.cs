using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Matchers;

/// <summary>Evaluates a chain of <see cref="IPoemMatcher"/> instances to detect the poem type.</summary>
internal sealed class PoemMatcherChain : IPoemMatcherChain
{
    private readonly IPoemMatcher[] _matchers;

    /// <summary>Initializes a new instance of the <see cref="PoemMatcherChain"/> class.</summary>
    /// <param name="matchers">The matchers to evaluate, which will be ordered by <see cref="IPoemMatcher.Priority"/>.</param>
    public PoemMatcherChain(IEnumerable<IPoemMatcher> matchers)
    {
        _matchers = matchers.OrderBy(m => m.Priority).ToArray();
    }

    /// <inheritdoc/>
    public PoemType Match(string[] lines, int[] syllableCounts)
    {
        foreach (var matcher in _matchers)
        {
            var result = matcher.TryMatch(lines, syllableCounts);
            if (result.HasValue)
            {
                return result.Value;
            }
        }

        return PoemType.Freeform;
    }
}
