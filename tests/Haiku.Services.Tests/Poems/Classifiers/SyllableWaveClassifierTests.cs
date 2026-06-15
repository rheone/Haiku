namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableWaveClassifierTests
{
    private readonly SyllableWaveClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid 5-line wave pattern.</summary>
    public void Match_WithValidWave_ReturnsDefinition_Test()
    {
        var lines = new[] { "a", "bb", "ccc", "bb", "a" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-wave");
    }

    [Fact]
    /// <summary>Verifies the classifier identifies a valid 7-line wave pattern.</summary>
    public void Match_WithValid7LineWave_ReturnsDefinition_Test()
    {
        var lines = new[] { "aa", "bbb", "cccc", "ddddd", "cccc", "bbb", "aa" };
        var counts = new[] { 2, 3, 4, 5, 4, 3, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-wave");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 5 lines.</summary>
    public void NoMatch_WithLessThan5Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "bb", "a" };
        var counts = new[] { 1, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects asymmetric (non-palindromic) sequences.</summary>
    public void NoMatch_WithAsymmetricWave_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "bb", "ccc", "dddd", "eeeee" };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2200.</summary>
    public void Priority_Is2200_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2200);
    }

    #endregion
}
