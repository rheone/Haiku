namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordFibClassifierTests
{
    private readonly WordFibClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid Fibonacci word-count pattern.</summary>
    public void Match_WithValidFibDigits_ReturnsDefinition_Test()
    {
        var lines = new[] { "a", "a", "a b", "a b c", "a b c d e" };
        var counts = new[] { 1, 1, 2, 3, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-fib");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects non-Fibonacci word-count sequences.</summary>
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
        var lines = new[] { "a", "a" };
        var counts = new[] { 1, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 1900.</summary>
    public void Priority_Is1900_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 1900);
    }

    #endregion
}
