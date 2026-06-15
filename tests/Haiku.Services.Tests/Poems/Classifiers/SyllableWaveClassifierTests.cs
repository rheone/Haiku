namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableWaveClassifierTests
{
    private readonly SyllableWaveClassifier _classifier = new();

    [Fact]
    public void Match_WithValidWave_ReturnsDefinition()
    {
        var lines = new[] { "a", "bb", "ccc", "bb", "a" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-wave");
    }

    [Fact]
    public void Match_WithValid7LineWave_ReturnsDefinition()
    {
        var lines = new[] { "aa", "bbb", "cccc", "ddddd", "cccc", "bbb", "aa" };
        var counts = new[] { 2, 3, 4, 5, 4, 3, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-wave");
    }

    [Fact]
    public void NoMatch_WithLessThan5Lines_ReturnsFalse()
    {
        var lines = new[] { "a", "bb", "a" };
        var counts = new[] { 1, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithAsymmetricWave_ReturnsFalse()
    {
        var lines = new[] { "a", "bb", "ccc", "dddd", "eeeee" };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is2200()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2200);
    }
}
