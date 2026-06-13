namespace Haiku.Services.Tests.Poems.Matchers;

public class HaikuMatcherTests
{
    private readonly HaikuMatcher _matcher = new();

    [Fact]
    public void TryMatch_FiveSevenFive_ReturnsHaiku()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 7, 5]);
        Assert.Equal(PoemType.Haiku, result);
    }

    [Fact]
    public void TryMatch_FiveSevenSix_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 7, 6]);
        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_TwoLines_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two"], [5, 7]);
        Assert.Null(result);
    }
}
