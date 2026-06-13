namespace Haiku.Services.Tests.Poems.Matchers;

public class CompressedMatcherTests
{
    private readonly CompressedMatcher _matcher = new();

    [Fact]
    public void TryMatch_TwoThreeTwo_ReturnsCompressed()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [2, 3, 2]);
        Assert.Equal(PoemType.Compressed, result);
    }

    [Fact]
    public void TryMatch_ThreeFiveThree_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [3, 5, 3]);
        Assert.Null(result);
    }
}
