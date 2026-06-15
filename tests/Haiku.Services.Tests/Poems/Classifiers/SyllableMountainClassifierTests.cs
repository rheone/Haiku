namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableMountainClassifierTests
{
    private readonly SyllableMountainClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid mountain pattern (strictly increasing by 1).</summary>
    public void Match_WithValidMountain_ReturnsDefinition_Test()
    {
        var lines = new[] { "a", "aa", "bbb", "bbbb", "ccccc" };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-mountain");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects when first line is not 1.</summary>
    public void NoMatch_FirstLineNot1_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "bbb", "bbbb" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects a sequence with gaps.</summary>
    public void NoMatch_WithGap_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "bbb", "ccccc" };
        var counts = new[] { 1, 3, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "aa" };
        var counts = new[] { 1, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3800.</summary>
    public void Priority_Is3800_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3800);
    }

    #endregion
}
