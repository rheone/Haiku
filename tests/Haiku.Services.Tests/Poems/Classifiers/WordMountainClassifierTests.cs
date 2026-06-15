namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordMountainClassifierTests
{
    private readonly WordMountainClassifier _classifier = new();

    [Fact]
    public void Match_WithValidMountain_ReturnsDefinition()
    {
        var lines = new[] { "a", "a b", "a b c", "a b c d", "a b c d e" };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-mountain");
    }

    [Fact]
    public void NoMatch_FirstLineNot1_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b c", "a b c d" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithGap_ReturnsFalse()
    {
        var lines = new[] { "a", "a b c", "a b c d e" };
        var counts = new[] { 1, 3, 5 };
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
    public void Priority_Is3900()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3900);
    }
}
