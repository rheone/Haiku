namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableErosionClassifierTests
{
    private readonly SyllableErosionClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid erosion pattern (strictly decreasing by 1).</summary>
    public void Match_WithValidErosion_ReturnsDefinition_Test()
    {
        var lines = new[] { "ccccc", "bbbb", "bbb", "aa", "a" };
        var counts = new[] { 5, 4, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-erosion");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a sequence with gaps.</summary>
    public void NoMatch_WithGap_ReturnsFalse_Test()
    {
        var lines = new[] { "ccccc", "bbb", "a" };
        var counts = new[] { 5, 3, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects when last line is not 1.</summary>
    public void NoMatch_LastLineNot1_ReturnsFalse_Test()
    {
        var lines = new[] { "bbbb", "bbb", "aa" };
        var counts = new[] { 4, 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "bbb", "aa" };
        var counts = new[] { 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3600.</summary>
    public void Priority_Is3600_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3600);
    }

    #endregion
}
