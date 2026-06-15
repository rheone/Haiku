namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordPulseClassifierTests
{
    private readonly WordPulseClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid pulse pattern (alternating counts).</summary>
    public void Match_WithValidPulse_ReturnsDefinition_Test()
    {
        var lines = new[] { "a b", "a b c d e", "a b", "a b c d e" };
        var counts = new[] { 2, 5, 2, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-pulse");
    }

    [Fact]
    /// <summary>Verifies the classifier matches a longer alternating pulse pattern.</summary>
    public void Match_WithLongerPulse_ReturnsDefinition_Test()
    {
        var lines = new[] { "a b c", "a b c d e f g", "a b c", "a b c d e f g", "a b c", "a b c d e f g" };
        var counts = new[] { 3, 7, 3, 7, 3, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-pulse");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern where all values are equal.</summary>
    public void NoMatch_WithSameValues_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b", "a b", "a b" };
        var counts = new[] { 2, 2, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects a pattern with an odd number of lines.</summary>
    public void NoMatch_WithOddLength_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c d e", "a b" };
        var counts = new[] { 2, 5, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 4 lines.</summary>
    public void NoMatch_WithLessThan4Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c d e" };
        var counts = new[] { 2, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3100.</summary>
    public void Priority_Is3100_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3100);
    }

    #endregion
}
