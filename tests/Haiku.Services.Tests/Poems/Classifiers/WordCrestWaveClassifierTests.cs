namespace Haiku.Services.Tests.Poems.Classifiers;

public class WordCrestWaveClassifierTests
{
    private readonly WordCrestWaveClassifier _classifier = new();

    [Fact]
    public void Match_WithValidCrestWave_ReturnsDefinition()
    {
        var lines = new[]
        {
            "one two",
            "three four five",
            "six seven eight nine",
            "three four five",
            "one two",
            "one",
            "two three",
            "four five six",
            "two three",
            "one",
        };
        var counts = new[] { 2, 3, 4, 3, 2, 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "word-crest-wave");
    }

    [Fact]
    public void NoMatch_WithWrongPeakOrder_ReturnsFalse()
    {
        var lines = new[]
        {
            "one",
            "two three",
            "four five six",
            "two three",
            "one",
            "one two",
            "three four five",
            "six seven eight nine",
            "three four five",
            "one two",
        };
        var counts = new[] { 1, 2, 3, 2, 1, 2, 3, 4, 3, 2 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithSingleWave_ReturnsFalse()
    {
        var lines = new[] { "one", "two three", "four five six", "two three", "one" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is2500()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2500);
    }
}
