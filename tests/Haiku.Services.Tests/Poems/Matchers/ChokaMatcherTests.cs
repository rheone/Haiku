namespace Haiku.Services.Tests.Poems.Matchers;

public class ChokaMatcherTests
{
    private readonly ChokaMatcher _matcher = new();

    [Fact]
    public void TryMatch_SevenLinesValidPattern_ReturnsChoka()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5", "l6", "l7"], [5, 7, 5, 7, 5, 7, 7]);
        Assert.Equal(PoemType.Choka, result);
    }

    [Fact]
    public void TryMatch_NineLinesValidPattern_ReturnsChoka()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5", "l6", "l7", "l8", "l9"], [5, 7, 5, 7, 5, 7, 5, 7, 7]);
        Assert.Equal(PoemType.Choka, result);
    }

    [Fact]
    public void TryMatch_FiveLines_ReturnsNull()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5"], [5, 7, 5, 7, 7]);
        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_SevenLinesWrongEnding_ReturnsNull()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5", "l6", "l7"], [5, 7, 5, 7, 5, 5, 7]);
        Assert.Null(result);
    }
}
