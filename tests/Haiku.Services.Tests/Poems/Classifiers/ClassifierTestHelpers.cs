using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

/// <summary>
/// Shared test utilities for classifier tests. Reduces boilerplate when
/// verifying that a classifier matches or rejects a given pattern.
/// </summary>
/// <remarks>
/// <para>
/// Usage:
/// <code>
/// // Arrange
/// var classifier = new SyllablePiClassifier();
/// var lines = new[] { "a", "bb", "ccc" };
/// var counts = new[] { 1, 4, 1 };
///
/// // Act / Assert (match)
/// ClassifierTestHelpers.AssertMatch(classifier, lines, counts, "syllable-pi");
///
/// // Act / Assert (no match)
/// ClassifierTestHelpers.AssertNoMatch(classifier, lines, new[] { 1, 2, 3 });
/// </code>
/// </para>
/// </remarks>
internal static class ClassifierTestHelpers
{
    /// <summary>
    /// Creates a <see cref="TokenizedLine"/> array from raw line strings.
    /// Each line is split on whitespace.
    /// </summary>
    public static TokenizedLine[] Tokenize(string[] lines)
    {
        return lines
            .Select(l =>
            {
                var words = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new TokenizedLine
                {
                    Words = words,
                    WordCount = words.Length,
                };
            })
            .ToArray();
    }

    /// <summary>
    /// Asserts that <paramref name="classifier"/> successfully classifies
    /// the given lines and returns a definition with <see cref="PoemDefinition.TypeId"/>
    /// equal to <paramref name="expectedTypeId"/>.
    /// </summary>
    public static void AssertMatch(
        IPoemClassifier classifier,
        string[] lines,
        int[] syllableCounts,
        string expectedTypeId)
    {
        var tokenized = Tokenize(lines);
        var success = classifier.TryClassify(lines, syllableCounts, tokenized, out var definition);

        Assert.True(success, $"Expected {classifier.GetType().Name} to match.");
        Assert.NotNull(definition);
        Assert.Equal(expectedTypeId, definition!.TypeId);
    }

    /// <summary>
    /// Asserts that <paramref name="classifier"/> does NOT classify the
    /// given lines with the given <paramref name="syllableCounts"/>.
    /// </summary>
    public static void AssertNoMatch(
        IPoemClassifier classifier,
        string[] lines,
        int[] syllableCounts)
    {
        var tokenized = Tokenize(lines);
        var success = classifier.TryClassify(lines, syllableCounts, tokenized, out _);

        Assert.False(success, $"Expected {classifier.GetType().Name} to NOT match.");
    }

    /// <summary>
    /// Asserts that <paramref name="classifier"/> has the expected priority.
    /// </summary>
    public static void AssertPriority(IPoemClassifier classifier, int expectedPriority)
    {
        Assert.Equal(expectedPriority, classifier.Priority);
    }
}
