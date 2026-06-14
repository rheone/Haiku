namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.AmericanLuneMatcher"/> — verifying
/// the 3-5-3 syllable pattern is correctly identified as an American lune
/// and that patterns like 5-7-5 (haiku) are rejected.
/// </summary>
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
