using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableErosionClassifierTests
{
    private readonly SyllableErosionClassifier _classifier = new();

    [Fact]
    public void Match_WithValidErosion_ReturnsDefinition()
    {
        var lines = new[] { "ccccc", "bbbb", "bbb", "aa", "a" };
        var counts = new[] { 5, 4, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-erosion");
    }

    [Fact]
    public void NoMatch_WithGap_ReturnsFalse()
    {
        var lines = new[] { "ccccc", "bbb", "a" };
        var counts = new[] { 5, 3, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_LastLineNot1_ReturnsFalse()
    {
        var lines = new[] { "bbbb", "bbb", "aa" };
        var counts = new[] { 4, 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "bbb", "aa" };
        var counts = new[] { 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is3600()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3600);
    }
}
