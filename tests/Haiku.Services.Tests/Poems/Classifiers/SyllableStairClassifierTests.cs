namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableStairClassifierTests
{
    private readonly SyllableStairClassifier _classifier = new();

    [Fact]
    public void Match_WithValidStair_ReturnsDefinition()
    {
        var lines = new[] { "bbb", "bbbb", "ccccc", "bbbbbb", "bbbbbbb" };
        var counts = new[] { 3, 4, 5, 6, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-stair");
    }

    [Fact]
    public void NoMatch_WithGap_ReturnsFalse()
    {
        var lines = new[] { "bbb", "ccccc", "bbbbbbb" };
        var counts = new[] { 3, 5, 7 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "a", "aa" };
        var counts = new[] { 1, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3400()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3400);
    }
}
