namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.TankaMatcher"/> — verifying
/// the 5-7-5-7-7 five-line syllable pattern is correctly identified as a tanka
/// and that patterns with a different final line syllable count are rejected.
/// </summary>
public class TankaMatcherTests
{
    private readonly TankaMatcher _matcher = new();

    [Fact]
    public void TryMatch_FiveSevenFiveSevenSeven_ReturnsTanka()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three", "line four", "line five"], [5, 7, 5, 7, 7]);
        Assert.Equal(PoemType.Tanka, result);
    }

    [Fact]
    public void TryMatch_FiveSevenFiveSevenEight_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three", "line four", "line five"], [5, 7, 5, 7, 8]);
        Assert.Null(result);
    }
}
