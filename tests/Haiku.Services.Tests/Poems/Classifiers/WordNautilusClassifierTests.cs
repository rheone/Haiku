namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordNautilusClassifierTests
{
    private readonly WordNautilusClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid nautilus growth pattern.</summary>
    public void Match_WithValidNautilus_ReturnsDefinition_Test()
    {
        var lines = new[]
        {
            "a b",
            "a b c",
            "a b c d e",
            "a b c d e f g h",
            "a b c d e f g h i j k l",
            "a b c d e f g h i j k l m n o p q",
        };
        var counts = new[] { 2, 3, 5, 8, 12, 17 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-nautilus");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects non-nautilus growth sequences.</summary>
    public void NoMatch_WithWrongGrowth_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c", "a b c d" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects non-increasing sequences.</summary>
    public void NoMatch_NotIncreasing_ReturnsFalse_Test()
    {
        var lines = new[] { "a b c", "a b", "a b" };
        var counts = new[] { 3, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a b", "a b c" };
        var counts = new[] { 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 4100.</summary>
    public void Priority_Is4100_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 4100);
    }

    #endregion
}
