namespace Haiku.Services.Tests.Poems.Matchers;

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
