namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.KatautaMatcher"/> — verifying
/// the 5-7-7 syllable pattern is correctly identified as a katauta
/// and that patterns like 5-7-5 (haiku) are rejected.
/// </summary>
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
