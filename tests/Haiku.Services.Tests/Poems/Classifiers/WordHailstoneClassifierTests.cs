using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordHailstoneClassifierTests
{
    private readonly WordHailstoneClassifier _classifier = new();

    [Fact]
    public void Match_WithValidCollatz_ReturnsDefinition()
    {
        var lines = new[] { "a b c", "a b c d e f g h i j", "a b c d e", "a b c d e f g h i j k l m n o p", "a b c d e f g h", "a b c d", "a b", "a" };
        var counts = new[] { 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-hailstone");
    }

    [Fact]
    public void Match_ShorterCollatz_ReturnsDefinition()
    {
        var lines = new[] { "a b c d e f", "a b c", "a b c d e f g h i j", "a b c d e", "a b c d e f g h i j k l m n o p", "a b c d e f g h", "a b c d", "a b", "a" };
        var counts = new[] { 6, 3, 10, 5, 16, 8, 4, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-hailstone");
    }

    [Fact]
    public void NoMatch_LastLineNot1_ReturnsFalse()
    {
        var lines = new[] { "a b c", "a b c d e f g h i j", "a b c d e" };
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
    public void Priority_Is3300()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 3300);
    }
}
