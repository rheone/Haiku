using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordReverseFibClassifierTests
{
    private readonly WordReverseFibClassifier _classifier = new();

    [Fact]
    public void Match_WithValidReverseFib_ReturnsDefinition()
    {
        var lines = new[] { "a b c d e", "a b c", "a b", "a", "a" };
        var counts = new[] { 5, 3, 2, 1, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-reverse-fib");
    }

    [Fact]
    public void NoMatch_WithWrongDigits_ReturnsFalse()
    {
        var lines = new[] { "a", "a b", "a b c" };
        var counts = new[] { 1, 2, 3 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is2100()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2100);
    }
}
