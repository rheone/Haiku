namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableStairClassifierTests
{
    private readonly SyllableStairClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid stair pattern (strictly increasing by 1).</summary>
    public void Match_WithValidStair_ReturnsDefinition_Test()
    {
        var lines = new[] { "bbb", "bbbb", "ccccc", "bbbbbb", "bbbbbbb" };
        var counts = new[] { 3, 4, 5, 6, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-stair");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a sequence with gaps.</summary>
    public void NoMatch_WithGap_ReturnsFalse_Test()
    {
        var lines = new[] { "bbb", "ccccc", "bbbbbbb" };
        var counts = new[] { 3, 5, 7 };
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
    /// <summary>Verifies the classifier priority is 3400.</summary>
    public void Priority_Is3400_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3400);
    }

    #endregion
}
