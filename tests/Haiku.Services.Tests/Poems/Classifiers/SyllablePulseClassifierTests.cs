namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllablePulseClassifierTests
{
    private readonly SyllablePulseClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid pulse pattern (alternating values).</summary>
    public void Match_WithValidPulse_ReturnsDefinition_Test()
    {
        var lines = new[] { "aa", "ccccc", "aa", "ccccc" };
        var counts = new[] { 2, 5, 2, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-pulse");
    }

    [Fact]
    /// <summary>Verifies the classifier identifies a longer pulse pattern.</summary>
    public void Match_WithLongerPulse_ReturnsDefinition_Test()
    {
        var lines = new[] { "bbb", "bbbbbbb", "bbb", "bbbbbbb", "bbb", "bbbbbbb" };
        var counts = new[] { 3, 7, 3, 7, 3, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-pulse");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects sequences where all values are the same.</summary>
    public void NoMatch_WithSameValues_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "aa", "aa", "aa" };
        var counts = new[] { 2, 2, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with an odd number of lines.</summary>
    public void NoMatch_WithOddLength_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "ccccc", "aa" };
        var counts = new[] { 2, 5, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 4 lines.</summary>
    public void NoMatch_WithLessThan4Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "ccccc" };
        var counts = new[] { 2, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3000.</summary>
    public void Priority_Is3000_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3000);
    }

    #endregion
}
