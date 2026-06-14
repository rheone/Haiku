using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordNautilusClassifierTests
{
    private readonly WordNautilusClassifier _classifier = new();

    [Fact]
    public void Match_WithValidNautilus_ReturnsDefinition()
    {
        var lines = new[] { "a b", "a b c", "a b c d e", "a b c d e f g h", "a b c d e f g h i j k l", "a b c d e f g h i j k l m n o p q" };
        var counts = new[] { 2, 3, 5, 8, 12, 17 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-nautilus");
    }

    [Fact]
    public void NoMatch_WithWrongGrowth_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b c", "a b c d" };
        var counts = new[] { 2, 3, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_NotIncreasing_ReturnsFalse()
    {
        var lines = new[] { "a b c", "a b", "a b" };
        var counts = new[] { 3, 2, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b c" };
        var counts = new[] { 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is4100()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 4100);
    }
}
