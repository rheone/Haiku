namespace Haiku.Services.Tests.Poems.Matchers;

public class ReverseCinquainMatcherTests
{
    private readonly ReverseCinquainMatcher _matcher = new();

    [Fact]
    public void TryMatch_TwoEightSixFourTwo_ReturnsReverseCinquain()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three", "line four", "line five"], [2, 8, 6, 4, 2]);
        Assert.Equal(PoemType.ReverseCinquain, result);
    }

    [Fact]
    public void TryMatch_TwoEightSixFourThree_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three", "line four", "line five"], [2, 8, 6, 4, 3]);
        Assert.Null(result);
    }
}
