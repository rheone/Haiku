using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class SyllableCrashWaveClassifierTests
{
    private readonly SyllableCrashWaveClassifier _classifier = new();

    [Fact]
    public void Match_WithValidCrashWave_ReturnsDefinition()
    {
        var lines = new[] { "a", "bb", "ccc", "bb", "a", "aa", "bbb", "cccc", "bbb", "aa" };
        var counts = new[] { 1, 2, 3, 2, 1, 2, 3, 4, 3, 2 };
        ClassifierTestHelpers.AssertMatch(_classifier, lines, counts, "syllable-crash-wave");
    }

    [Fact]
    public void NoMatch_WithWrongPeakOrder_ReturnsFalse()
    {
        var lines = new[] { "aa", "bbb", "cccc", "bbb", "aa", "a", "bb", "ccc", "bb", "a" };
        var counts = new[] { 2, 3, 4, 3, 2, 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void NoMatch_WithSingleWave_ReturnsFalse()
    {
        var lines = new[] { "a", "bb", "ccc", "bb", "a" };
        var counts = new[] { 1, 2, 3, 2, 1 };
        ClassifierTestHelpers.AssertNoMatch(_classifier, lines, counts);
    }

    [Fact]
    public void Priority_Is2600()
    {
        ClassifierTestHelpers.AssertPriority(_classifier, 2600);
    }
}
