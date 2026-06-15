namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordErosionClassifierTests
{
    private readonly WordErosionClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid erosion pattern (decreasing by 1).</summary>
    public void Match_WithValidErosion_ReturnsDefinition_Test()
    {
        var lines = new[] { "a b c d e", "a b c d", "a b c", "a b", "a" };
        var counts = new[] { 5, 4, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-erosion");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern with gaps in descending counts.</summary>
    public void NoMatch_WithGap_ReturnsFalse_Test()
    {
        var lines = new[] { "a b c d e", "a b c", "a" };
        var counts = new[] { 5, 3, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern that does not end at 1.</summary>
    public void NoMatch_LastLineNot1_ReturnsFalse_Test()
    {
        var lines = new[] { "a b c d", "a b c", "a b" };
        var counts = new[] { 4, 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a b c", "a b" };
        var counts = new[] { 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3700.</summary>
    public void Priority_Is3700_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3700);
    }

    #endregion
}
