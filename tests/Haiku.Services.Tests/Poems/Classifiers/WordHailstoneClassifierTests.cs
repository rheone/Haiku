namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordHailstoneClassifierTests
{
    private readonly WordHailstoneClassifier _classifier = new();

    #region Match

    [Fact]
    /// <summary>Verifies the classifier matches a valid Collatz hailstone sequence.</summary>
    public void Match_WithValidCollatz_ReturnsDefinition_Test()
    {
        var lines = new[]
        {
            "a b c",
            "a b c d e f g h i j",
            "a b c d e",
            "a b c d e f g h i j k l m n o p",
            "a b c d e f g h",
            "a b c d",
            "a b",
            "a",
        };
        var counts = new[] { 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-hailstone");
    }

    [Fact]
    /// <summary>Verifies the classifier matches a shorter valid Collatz sequence.</summary>
    public void Match_ShorterCollatz_ReturnsDefinition_Test()
    {
        var lines = new[]
        {
            "a b c d e f",
            "a b c",
            "a b c d e f g h i j",
            "a b c d e",
            "a b c d e f g h i j k l m n o p",
            "a b c d e f g h",
            "a b c d",
            "a b",
            "a",
        };
        var counts = new[] { 6, 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-hailstone");
    }

    #endregion

    #region NoMatch

    [Fact]
    /// <summary>Verifies the classifier rejects a sequence that does not end at 1.</summary>
    public void NoMatch_LastLineNot1_ReturnsFalse_Test()
    {
        var lines = new[] { "a b c", "a b c d e f g h i j", "a b c d e" };
        var counts = new[] { 3, 10, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    /// <summary>Verifies the classifier rejects patterns with fewer than 3 lines.</summary>
    public void NoMatch_WithLessThan3Lines_ReturnsFalse_Test()
    {
        var lines = new[] { "a", "a" };
        var counts = new[] { 1, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    #endregion

    #region Priority

    [Fact]
    /// <summary>Verifies the classifier priority is 3300.</summary>
    public void Priority_Is3300_Test()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3300);
    }

    #endregion
}
