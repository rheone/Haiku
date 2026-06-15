namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableReverseFibClassifierTests
{
    private readonly SyllableReverseFibClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid reverse-Fibonacci syllable pattern.</summary>
    public void Match_WithValidReverseFib_ReturnsDefinition_Test()
    {
        var lines = new[] { "a b c d e", "a b c", "a b", "a", "a" };
        var counts = new[] { 5, 3, 2, 1, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-reverse-fib");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects non-reverse-Fibonacci counts.</summary>
    public void NoMatch_WithWrongDigits_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a b", "a b c" };
        var counts = new[] { 1, 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2000.</summary>
    public void Priority_Is2000_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2000);
    }

    #endregion
}
