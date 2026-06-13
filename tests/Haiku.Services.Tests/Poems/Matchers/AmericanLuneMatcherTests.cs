namespace Haiku.Services.Tests.Poems.Matchers;

public class AmericanLuneMatcherTests
{
    private readonly AmericanLuneMatcher _matcher = new();

    [Fact]
    public void TryMatch_ThreeFiveThree_ReturnsAmericanLune()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [3, 5, 3]);
        Assert.Equal(PoemType.AmericanLune, result);
    }

    [Fact]
    public void TryMatch_FiveSevenFive_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 7, 5]);
        Assert.Null(result);
    }
}
