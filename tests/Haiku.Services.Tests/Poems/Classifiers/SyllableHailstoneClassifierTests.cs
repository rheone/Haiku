namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableHailstoneClassifierTests
{
    private readonly SyllableHailstoneClassifier _classifier = new();

    [Fact]
    public void Match_WithValidCollatz_ReturnsDefinition()
    {
        var lines = new[] { "bbb", "aaaaaaaaaa", "ccccc", "bbbbbbbbbbbbbbbb", "aaaaaaaa", "bbbb", "aa", "a" };
        var counts = new[] { 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-hailstone");
    }

    [Fact]
    public void Match_ShorterCollatz_ReturnsDefinition()
    {
        var lines = new[] { "bbbbbb", "bbb", "aaaaaaaaaa", "ccccc", "bbbbbbbbbbbbbbbb", "aaaaaaaa", "bbbb", "aa", "a" };
        var counts = new[] { 6, 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-hailstone");
    }

    [Fact]
    public void NoMatch_LastLineNot1_ReturnsFalse()
    {
        var lines = new[] { "bbb", "aaaaaaaaaa", "ccccc" };
        var counts = new[] { 3, 10, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "a", "a" };
        var counts = new[] { 1, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3200()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3200);
    }
}
