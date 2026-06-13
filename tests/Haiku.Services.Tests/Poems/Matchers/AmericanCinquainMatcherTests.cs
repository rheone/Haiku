namespace Haiku.Services.Tests.Poems.Matchers;

public class AmericanCinquainMatcherTests
{
    private readonly AmericanCinquainMatcher _matcher = new();

    [Fact]
    public void TryMatch_TwoFourSixEightTwo_ReturnsAmericanCinquain()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three", "line four", "line five"], [2, 4, 6, 8, 2]);
        Assert.Equal(PoemType.AmericanCinquain, result);
    }

    [Fact]
    public void TryMatch_TwoFourSixEightThree_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three", "line four", "line five"], [2, 4, 6, 8, 3]);
        Assert.Null(result);
    }
}
