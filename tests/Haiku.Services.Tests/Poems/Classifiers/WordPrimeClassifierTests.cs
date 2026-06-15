namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordPrimeClassifierTests
{
    private readonly WordPrimeClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid prime-number word-count pattern.</summary>
    public void Match_WithAllPrimes_ReturnsDefinition_Test()
    {
        var lines = new[] { "a b", "a b c", "a b c d e", "a b c d e f g", "a b c d e f g h i j k" };
        var counts = new[] { 2, 3, 5, 7, 11 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-prime");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects sequences containing a non-prime count.</summary>
    public void NoMatch_WithNonPrime_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c d", "a b c d e" };
        var counts = new[] { 2, 4, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c" };
        var counts = new[] { 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2900.</summary>
    public void Priority_Is2900_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2900);
    }

    #endregion
}
