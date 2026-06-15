namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordWaveClassifierTests
{
    private readonly WordWaveClassifier _classifier = new();

    [Fact]
    public void Match_WithValidWave_ReturnsDefinition()
    {
        var lines = new[] { "one", "two three", "four five six", "seven eight", "nine" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-wave");
    }

    [Fact]
    public void Match_WithValid7LineWave_ReturnsDefinition()
    {
        var lines = new[]
        {
            "one two",
            "three four five",
            "six seven eight nine",
            "ten eleven twelve thirteen fourteen",
            "fifteen sixteen seventeen eighteen",
            "nineteen twenty twenty-one",
            "twenty-two twenty-three",
        };
        var counts = new[] { 2, 3, 4, 5, 4, 3, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-wave");
    }

    [Fact]
    public void NoMatch_WithLessThan5Lines_ReturnsFalse()
    {
        var lines = new[] { "one", "two three", "four" };
        var counts = new[] { 1, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithAsymmetricWave_ReturnsFalse()
    {
        var lines = new[]
        {
            "one",
            "two three",
            "four five six",
            "seven eight nine ten",
            "eleven twelve thirteen fourteen fifteen",
        };
        var counts = new[] { 1, 2, 3, 4, 5 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is2300()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2300);
    }
}
