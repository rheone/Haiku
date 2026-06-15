namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordStairClassifierTests
{
    private readonly WordStairClassifier _classifier = new();

    [Fact]
    public void Match_WithValidStair_ReturnsDefinition()
    {
        var lines = new[] { "a b c", "a b c d", "a b c d e", "a b c d e f", "a b c d e f g" };
        var counts = new[] { 3, 4, 5, 6, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-stair");
    }

    [Fact]
    public void NoMatch_WithGap_ReturnsFalse()
    {
        var lines = new[] { "a b c", "a b c d e", "a b c d e f g" };
        var counts = new[] { 3, 5, 7 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "a", "a b" };
        var counts = new[] { 1, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3500()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3500);
    }
}
