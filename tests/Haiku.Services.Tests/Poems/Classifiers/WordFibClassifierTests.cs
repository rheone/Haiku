using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordFibClassifierTests
{
    private readonly WordFibClassifier _classifier = new();

    [Fact]
    public void Match_WithValidFibDigits_ReturnsDefinition()
    {
        var lines = new[] { "a", "a", "a b", "a b c", "a b c d e" };
        var counts = new[] { 1, 1, 2, 3, 5 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-fib");
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
        var lines = new[] { "a", "a" };
        var counts = new[] { 1, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is1900()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 1900);
    }
}
