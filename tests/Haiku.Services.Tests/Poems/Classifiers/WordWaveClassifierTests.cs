namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordWaveClassifierTests
{
    private readonly WordWaveClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid symmetric wave pattern.</summary>
    public void Match_WithValidWave_ReturnsDefinition_Test()
    {
        var lines = new[] { "one", "two three", "four five six", "seven eight", "nine" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-wave");
    }

    [Fact]
    /// <summary>Verifies the classifier matches a valid 7-line symmetric wave pattern.</summary>
    public void Match_WithValid7LineWave_ReturnsDefinition_Test()
    {
        var lines = new[]
        {
            "one two",
            "three four five",
            "six seven eight nine",
            "ten eleven twelve thirteen fourteen",
            "fifteen sixteen seventeen eighteen",
            "nineteen twenty twenty-one",
            "twenty-two twenty-three",
        };
        var counts = new[] { 2, 3, 4, 5, 4, 3, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-wave");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 5 lines.</summary>
    public void NoMatch_WithLessThan5Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "one", "two three", "four" };
        var counts = new[] { 1, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects asymmetric (non-symmetrical) wave patterns.</summary>
    public void NoMatch_WithAsymmetricWave_ReturnsFalse_Test()
    {
        var lines = new[]
        {
            "one",
            "two three",
            "four five six",
            "seven eight nine ten",
            "eleven twelve thirteen fourteen fifteen",
        };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 2300.</summary>
    public void Priority_Is2300_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2300);
    }

    #endregion
}
