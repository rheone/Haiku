namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllablePulseClassifierTests
{
    private readonly SyllablePulseClassifier _classifier = new();

    [Fact]
    public void Match_WithValidPulse_ReturnsDefinition()
    {
        var lines = new[] { "aa", "ccccc", "aa", "ccccc" };
        var counts = new[] { 2, 5, 2, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-pulse");
    }

    [Fact]
    public void Match_WithLongerPulse_ReturnsDefinition()
    {
        var lines = new[] { "bbb", "bbbbbbb", "bbb", "bbbbbbb", "bbb", "bbbbbbb" };
        var counts = new[] { 3, 7, 3, 7, 3, 7 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-pulse");
    }

    [Fact]
    public void NoMatch_WithSameValues_ReturnsFalse()
    {
        var lines = new[] { "aa", "aa", "aa", "aa" };
        var counts = new[] { 2, 2, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithOddLength_ReturnsFalse()
    {
        var lines = new[] { "aa", "ccccc", "aa" };
        var counts = new[] { 2, 5, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan4Lines_ReturnsFalse()
    {
        var lines = new[] { "aa", "ccccc" };
        var counts = new[] { 2, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3000()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3000);
    }
}
