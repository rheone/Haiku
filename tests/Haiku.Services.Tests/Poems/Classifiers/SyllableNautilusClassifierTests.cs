using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableNautilusClassifierTests
{
    private readonly SyllableNautilusClassifier _classifier = new();

    [Fact]
    public void Match_WithValidNautilus_ReturnsDefinition()
    {
        var lines = new[] { "aa", "bbb", "ccccc", "aaaaaaaa", "aaaaaaaaaaaa", "bbbbbbbbbbbbbbbbb" };
        var counts = new[] { 2, 3, 5, 8, 12, 17 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-nautilus");
    }

    [Fact]
    public void NoMatch_WithWrongGrowth_ReturnsFalse()
    {
        var lines = new[] { "aa", "bbb", "bbbb" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_NotIncreasing_ReturnsFalse()
    {
        var lines = new[] { "bbb", "aa", "aa" };
        var counts = new[] { 3, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "aa", "bbb" };
        var counts = new[] { 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is4000()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 4000);
    }
}
