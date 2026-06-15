namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableNautilusClassifierTests
{
    private readonly SyllableNautilusClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier identifies a valid Nautilus pattern (+1, +2, +3, +4, +5 growth).</summary>
    public void Match_WithValidNautilus_ReturnsDefinition_Test()
    {
        var lines = new[] { "aa", "bbb", "ccccc", "aaaaaaaa", "aaaaaaaaaaaa", "bbbbbbbbbbbbbbbbb" };
        var counts = new[] { 2, 3, 5, 8, 12, 17 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-nautilus");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects counts not following Nautilus growth.</summary>
    public void NoMatch_WithWrongGrowth_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "bbb", "bbbb" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects non-increasing sequences.</summary>
    public void NoMatch_NotIncreasing_ReturnsFalse_Test()
    {
        var lines = new[] { "bbb", "aa", "aa" };
        var counts = new[] { 3, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects sequences with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "aa", "bbb" };
        var counts = new[] { 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 4000.</summary>
    public void Priority_Is4000_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 4000);
    }

    #endregion
}
