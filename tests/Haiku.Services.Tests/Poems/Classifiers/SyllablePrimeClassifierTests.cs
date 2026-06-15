namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllablePrimeClassifierTests
{
    private readonly SyllablePrimeClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid prime-number syllable pattern.</summary>
    public void Match_WithAllPrimes_ReturnsDefinition_Test()
    {
        var lines = new[] { "aa", "bbb", "ccccc", "bbbbbbb", "aaaaaaaaaaa" };
        var counts = new[] { 2, 3, 5, 7, 11 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-prime");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects sequences containing non-prime counts.</summary>
    public void NoMatch_WithNonPrime_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "bbbb", "ccccc" };
        var counts = new[] { 2, 4, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "bbb" };
        var counts = new[] { 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2800.</summary>
    public void Priority_Is2800_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2800);
    }

    #endregion
}
