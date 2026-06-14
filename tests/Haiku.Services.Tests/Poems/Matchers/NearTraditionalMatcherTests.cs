namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.NearTraditionalMatcher"/> — verifying
/// the 4-6-4 syllable pattern is correctly identified as near-traditional haiku
/// and that standard 5-7-5 patterns are rejected.
/// </summary>
public class NearTraditionalMatcherTests
{
    private readonly NearTraditionalMatcher _matcher = new();

    [Fact]
    public void TryMatch_FourSixFour_ReturnsNearTraditional()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [4, 6, 4]);
        Assert.Equal(PoemType.NearTraditional, result);
    }

    [Fact]
    public void TryMatch_FiveSevenFive_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 7, 5]);
        Assert.Null(result);
    }
}
