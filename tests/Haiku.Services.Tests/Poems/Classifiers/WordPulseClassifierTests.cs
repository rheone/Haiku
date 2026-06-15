namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordPulseClassifierTests
{
    private readonly WordPulseClassifier _classifier = new();

    [Fact]
    public void Match_WithValidPulse_ReturnsDefinition()
    {
        var lines = new[] { "a b", "a b c d e", "a b", "a b c d e" };
        var counts = new[] { 2, 5, 2, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-pulse");
    }

    [Fact]
    public void Match_WithLongerPulse_ReturnsDefinition()
    {
        var lines = new[] { "a b c", "a b c d e f g", "a b c", "a b c d e f g", "a b c", "a b c d e f g" };
        var counts = new[] { 3, 7, 3, 7, 3, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-pulse");
    }

    [Fact]
    public void NoMatch_WithSameValues_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b", "a b", "a b" };
        var counts = new[] { 2, 2, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithOddLength_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b c d e", "a b" };
        var counts = new[] { 2, 5, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan4Lines_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b c d e" };
        var counts = new[] { 2, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3100()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3100);
    }
}
