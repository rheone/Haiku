namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.IsosyllabicMatcher"/> — verifying
/// that poems with equal syllable counts per line are identified as isosyllabic
/// regardless of line count, and that unequal syllable patterns are rejected.
/// </summary>
public class IsosyllabicMatcherTests
{
    private readonly IsosyllabicMatcher _matcher = new();

    [Fact]
    public void TryMatch_ThreeLinesEqualSyllables_ReturnsIsosyllabic()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [4, 4, 4]);
        Assert.Equal(PoemType.Isosyllabic, result);
    }

    [Fact]
    public void TryMatch_TwoLinesEqualSyllables_ReturnsIsosyllabic()
    {
        var result = _matcher.TryMatch(["line one", "line two"], [5, 5]);
        Assert.Equal(PoemType.Isosyllabic, result);
    }

    [Fact]
    public void TryMatch_ThreeLinesUnequalSyllables_ReturnsNull()
    {
        var result = _matcher.TryMatch(["line one", "line two", "line three"], [4, 4, 5]);
        Assert.Null(result);
    }
}
