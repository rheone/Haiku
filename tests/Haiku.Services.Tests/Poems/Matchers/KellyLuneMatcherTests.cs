namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.KellyLuneMatcher"/> — verifying
/// the 5-3-5 syllable pattern is correctly identified as a Kelly lune
/// and that patterns like 3-5-3 (American lune) are rejected.
/// </summary>
public class KellyLuneMatcherTests
{
    private readonly KellyLuneMatcher _matcher = new();

    [Fact]
    public void TryMatch_FiveThreeFive_ReturnsKellyLune()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [5, 3, 5]);
        Assert.Equal(PoemType.KellyLune, result);
    }

    [Fact]
    public void TryMatch_ThreeFiveThree_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [3, 5, 3]);
        Assert.Null(result);
    }
}
