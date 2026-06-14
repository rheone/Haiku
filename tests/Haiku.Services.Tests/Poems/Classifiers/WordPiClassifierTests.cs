using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordPiClassifierTests
{
    private readonly WordPiClassifier _classifier = new();

    [Fact]
    public void Match_WithValidPiDigits_ReturnsDefinition()
    {
        var lines = new[] { "a", "a b c d", "a", "a b c d e", "a b c d e f g h i", "a b" };
        var counts = new[] { 1, 4, 1, 5, 9, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-pi");
    }

    [Fact]
    public void NoMatch_WithWrongDigits_ReturnsFalse()
    {
        var lines = new[] { "a", "a b", "a b c" };
        var counts = new[] { 1, 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithLessThan3Lines_ReturnsFalse()
    {
        var lines = new[] { "a", "a b c d" };
        var counts = new[] { 1, 4 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is1700()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 1700);
    }
}
