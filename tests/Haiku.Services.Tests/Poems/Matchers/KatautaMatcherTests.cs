namespace Haiku.Services.Tests.Poems.Matchers;

public class KatautaMatcherTests
{
    private readonly KatautaMatcher _matcher = new();

    [Fact]
    public void TryMatch_FiveSevenSeven_ReturnsKatauta()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 7, 7]);
        Assert.Equal(PoemType.Katauta, result);
    }

    [Fact]
    public void TryMatch_FiveSevenFive_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 7, 5]);
        Assert.Null(result);
    }
}
