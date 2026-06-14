using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordPrimeClassifierTests
{
    private readonly WordPrimeClassifier _classifier = new();

    [Fact]
    public void Match_WithAllPrimes_ReturnsDefinition()
    {
        var lines = new[] { "a b", "a b c", "a b c d e", "a b c d e f g", "a b c d e f g h i j k" };
        var counts = new[] { 2, 3, 5, 7, 11 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-prime");
    }

    [Fact]
    public void NoMatch_WithNonPrime_ReturnsFalse()
    {
        var lines = new[] { "a b", "a b c d", "a b c d e" };
        var counts = new[] { 2, 4, 5 };
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
    public void Priority_Is2900()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2900);
    }
}
