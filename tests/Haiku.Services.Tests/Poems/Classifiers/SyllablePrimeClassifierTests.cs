using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllablePrimeClassifierTests
{
    private readonly SyllablePrimeClassifier _classifier = new();

    [Fact]
    public void Match_WithAllPrimes_ReturnsDefinition()
    {
        var lines = new[] { "aa", "bbb", "ccccc", "bbbbbbb", "aaaaaaaaaaa" };
        var counts = new[] { 2, 3, 5, 7, 11 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-prime");
    }

    [Fact]
    public void NoMatch_WithNonPrime_ReturnsFalse()
    {
        var lines = new[] { "aa", "bbbb", "ccccc" };
        var counts = new[] { 2, 4, 5 };
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
    public void Priority_Is2800()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2800);
    }
}
