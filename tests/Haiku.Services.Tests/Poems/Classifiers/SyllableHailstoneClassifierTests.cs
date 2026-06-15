namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableHailstoneClassifierTests
{
    private readonly SyllableHailstoneClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid Collatz hailstone sequence.</summary>
    public void Match_WithValidCollatz_ReturnsDefinition_Test()
    {
        var lines = new[] { "bbb", "aaaaaaaaaa", "ccccc", "bbbbbbbbbbbbbbbb", "aaaaaaaa", "bbbb", "aa", "a" };
        var counts = new[] { 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-hailstone");
    }

    [Fact]
    /// <summary>Verifies the classifier identifies a shorter Collatz hailstone sequence.</summary>
    public void Match_ShorterCollatz_ReturnsDefinition_Test()
    {
        var lines = new[] { "bbbbbb", "bbb", "aaaaaaaaaa", "ccccc", "bbbbbbbbbbbbbbbb", "aaaaaaaa", "bbbb", "aa", "a" };
        var counts = new[] { 6, 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-hailstone");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects when last line is not 1.</summary>
    public void NoMatch_LastLineNot1_ReturnsFalse_Test()
    {
        var lines = new[] { "bbb", "aaaaaaaaaa", "ccccc" };
        var counts = new[] { 3, 10, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a" };
        var counts = new[] { 1, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3200.</summary>
    public void Priority_Is3200_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3200);
    }

    #endregion
}
