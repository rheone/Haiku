namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.SedokaMatcher"/> — verifying
/// the 5-7-7-5-7-7 six-line syllable pattern is correctly identified as a sedoka
/// and that deviating patterns are rejected.
/// </summary>
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
