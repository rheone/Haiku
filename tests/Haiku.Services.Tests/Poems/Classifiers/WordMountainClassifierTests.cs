namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordMountainClassifierTests
{
    private readonly WordMountainClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid mountain pattern (increasing by 1).</summary>
    public void Match_WithValidMountain_ReturnsDefinition_Test()
    {
        var lines = new[] { "a", "a b", "a b c", "a b c d", "a b c d e" };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-mountain");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern whose first line is not 1.</summary>
    public void NoMatch_FirstLineNot1_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c", "a b c d" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern with gaps in ascending counts.</summary>
    public void NoMatch_WithGap_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a b c", "a b c d e" };
        var counts = new[] { 1, 3, 5 };
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
    /// <summary>Verifies the classifier priority is 3900.</summary>
    public void Priority_Is3900_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3900);
    }

    #endregion
}
