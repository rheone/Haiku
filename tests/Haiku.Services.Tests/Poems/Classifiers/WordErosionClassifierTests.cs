namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordErosionClassifierTests
{
    private readonly WordErosionClassifier _classifier = new();

    [Fact]
    public void Match_WithValidErosion_ReturnsDefinition()
    {
        var lines = new[] { "a b c d e", "a b c d", "a b c", "a b", "a" };
        var counts = new[] { 5, 4, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-erosion");
    }

    [Fact]
    public void NoMatch_WithGap_ReturnsFalse()
    {
        var lines = new[] { "a b c d e", "a b c", "a" };
        var counts = new[] { 5, 3, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_LastLineNot1_ReturnsFalse()
    {
        var lines = new[] { "a b c d", "a b c", "a b" };
        var counts = new[] { 4, 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "a b c", "a b" };
        var counts = new[] { 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3700()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3700);
    }
}
