using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableMountainClassifierTests
{
    private readonly SyllableMountainClassifier _classifier = new();

    [Fact]
    public void Match_WithValidMountain_ReturnsDefinition()
    {
        var lines = new[] { "a", "aa", "bbb", "bbbb", "ccccc" };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-mountain");
    }

    [Fact]
    public void NoMatch_FirstLineNot1_ReturnsFalse()
    {
        var lines = new[] { "aa", "bbb", "bbbb" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithGap_ReturnsFalse()
    {
        var lines = new[] { "a", "bbb", "ccccc" };
        var counts = new[] { 1, 3, 5 };
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
    public void Priority_Is3800()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3800);
    }
}
