namespace Haiku.Services.Tests.Poems.Matchers;

public class SedokaMatcherTests
{
    private readonly SedokaMatcher _matcher = new();

    [Fact]
    public void TryMatch_FiveSevenSevenFiveSevenSeven_ReturnsSedoka()
    {
        var result = _matcher.TryMatch(
            ["line one", "line two", "line three", "line four", "line five", "line six"],
            [5, 7, 7, 5, 7, 7]
        );
        Assert.Equal(PoemType.Sedoka, result);
    }

    [Fact]
    public void TryMatch_FiveSevenFiveSevenSevenSeven_ReturnsNull()
    {
        var result = _matcher.TryMatch(
            ["line one", "line two", "line three", "line four", "line five", "line six"],
            [5, 7, 5, 7, 7, 7]
        );
        Assert.Null(result);
    }
}
