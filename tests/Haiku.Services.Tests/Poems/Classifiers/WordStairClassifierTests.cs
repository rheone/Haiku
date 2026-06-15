namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordStairClassifierTests
{
    private readonly WordStairClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid stair pattern (increasing by 1).</summary>
    public void Match_WithValidStair_ReturnsDefinition_Test()
    {
        var lines = new[] { "a b c", "a b c d", "a b c d e", "a b c d e f", "a b c d e f g" };
        var counts = new[] { 3, 4, 5, 6, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-stair");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern with gaps in ascending counts.</summary>
    public void NoMatch_WithGap_ReturnsFalse_Test()
    {
        var lines = new[] { "a b c", "a b c d e", "a b c d e f g" };
        var counts = new[] { 3, 5, 7 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a b" };
        var counts = new[] { 1, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3500.</summary>
    public void Priority_Is3500_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3500);
    }

    #endregion
}
