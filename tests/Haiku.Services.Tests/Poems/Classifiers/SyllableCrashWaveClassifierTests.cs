namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableCrashWaveClassifierTests
{
    private readonly SyllableCrashWaveClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid crash-wave pattern (double wave).</summary>
    public void Match_WithValidCrashWave_ReturnsDefinition_Test()
    {
        var lines = new[] { "a", "bb", "ccc", "bb", "a", "aa", "bbb", "cccc", "bbb", "aa" };
        var counts = new[] { 1, 2, 3, 2, 1, 2, 3, 4, 3, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-crash-wave");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a crest-wave pattern as not a crash-wave.</summary>
    public void NoMatch_WithWrongPeakOrder_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "bbb", "cccc", "bbb", "aa", "a", "bb", "ccc", "bb", "a" };
        var counts = new[] { 2, 3, 4, 3, 2, 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects a single wave (needs two waves).</summary>
    public void NoMatch_WithSingleWave_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "bb", "ccc", "bb", "a" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2600.</summary>
    public void Priority_Is2600_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2600);
    }

    #endregion
}
