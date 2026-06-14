namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.MonokuMatcher"/> — verifying
/// that single-line poems with 4-17 syllables are identified as monoku,
/// while poems below 4 or above 17 syllables, or with multiple lines, are rejected.
/// </summary>
public class MonokuMatcherTests
{
    private readonly MonokuMatcher _matcher = new();

    [Fact]
    public void TryMatch_OneLineFiveSyllables_ReturnsMonoku()
    {
        var result = _matcher.TryMatch(["a single line"], [5]);
        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public void TryMatch_OneLineFourSyllables_ReturnsMonoku()
    {
        var result = _matcher.TryMatch(["four syllables here"], [4]);
        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public void TryMatch_OneLineSeventeenSyllables_ReturnsMonoku()
    {
        var result = _matcher.TryMatch(["seventeen syllables here count"], [17]);
        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public void TryMatch_OneLineThreeSyllables_ReturnsNull()
    {
        var result = _matcher.TryMatch(["three syllables"], [3]);
        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_OneLineEighteenSyllables_ReturnsNull()
    {
        var result = _matcher.TryMatch(["eighteen syllables here count them all"], [18]);
        Assert.Null(result);
    }

    [Fact]
    public void TryMatch_TwoLines_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two"], [5, 7]);
        Assert.Null(result);
    }
}
