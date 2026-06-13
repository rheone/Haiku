namespace Haiku.Services.Tests.Poems.Matchers;

public class ButterflyCinquainMatcherTests
{
    private readonly ButterflyCinquainMatcher _matcher = new();

    [Fact]
    public void TryMatch_ValidPattern_ReturnsButterflyCinquain()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5", "l6", "l7", "l8", "l9"], [2, 4, 6, 8, 2, 8, 6, 4, 2]);
        Assert.Equal(PoemType.ButterflyCinquain, result);
    }

    [Fact]
    public void TryMatch_EightLines_ReturnsNull()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5", "l6", "l7", "l8"], [2, 4, 6, 8, 2, 8, 6, 4]);
        Assert.Null(result);
    }
}
