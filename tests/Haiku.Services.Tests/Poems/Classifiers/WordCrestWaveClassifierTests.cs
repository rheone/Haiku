namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordCrestWaveClassifierTests
{
    private readonly WordCrestWaveClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid double crest-wave pattern.</summary>
    public void Match_WithValidCrestWave_ReturnsDefinition_Test()
    {
        var lines = new[]
        {
            "one two",
            "three four five",
            "six seven eight nine",
            "three four five",
            "one two",
            "one",
            "two three",
            "four five six",
            "two three",
            "one",
        };
        var counts = new[] { 2, 3, 4, 3, 2, 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-crest-wave");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a crash-wave pattern (wrong peak order).</summary>
    public void NoMatch_WithWrongPeakOrder_ReturnsFalse_Test()
    {
        var lines = new[]
        {
            "one",
            "two three",
            "four five six",
            "two three",
            "one",
            "one two",
            "three four five",
            "six seven eight nine",
            "three four five",
            "one two",
        };
        var counts = new[] { 1, 2, 3, 2, 1, 2, 3, 4, 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects a single wave (needs two waves).</summary>
    public void NoMatch_WithSingleWave_ReturnsFalse_Test()
    {
        var lines = new[] { "one", "two three", "four five six", "two three", "one" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2500.</summary>
    public void Priority_Is2500_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2500);
    }

    #endregion
}
