namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordPiClassifierTests
{
    private readonly WordPiClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid pi-digit word-count pattern.</summary>
    public void Match_WithValidPiDigits_ReturnsDefinition_Test()
    {
        var lines = new[] { "a", "a b c d", "a", "a b c d e", "a b c d e f g h i", "a b" };
        var counts = new[] { 1, 4, 1, 5, 9, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-pi");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects non-pi digit sequences.</summary>
    public void NoMatch_WithWrongDigits_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a b", "a b c" };
        var counts = new[] { 1, 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a b c d" };
        var counts = new[] { 1, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 1700.</summary>
    public void Priority_Is1700_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 1700);
    }

    #endregion
}
