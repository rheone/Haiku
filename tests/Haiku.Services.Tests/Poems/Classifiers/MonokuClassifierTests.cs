using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class MonokuClassifierTests
{
    private readonly MonokuClassifier _classifier = new();

    private static TokenizedLine[] Tokenize(string[] lines)
    {
        return lines
            .Select(l => new TokenizedLine
            {
                Words = l.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                WordCount = l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            })
            .ToArray();
    }

    #region TryClassify

    /// <summary>
    /// Verifies that a single line with at least 4 syllables is correctly classified as Monoku.
    /// </summary>
    [Fact]
    public void TryClassify_SingleLineFiveSyllables_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "frog jumps in the old pond" };
        var syllableCounts = new[] { 5 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Monoku, definition!.Type);
        Assert.Equal(1, definition.LineCount);
        Assert.Equal([5], definition.SyllablesPerLine);
    }

    /// <summary>
    /// Verifies that a single line below the minimum syllable count returns false.
    /// </summary>
    [Fact]
    public void TryClassify_SingleLineBelowMinimum_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "hi there" };
        var syllableCounts = new[] { 3 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    /// <summary>
    /// Verifies that multiple lines returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongLineCount_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "some words", "more words" };
        var syllableCounts = new[] { 4, 4 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that MonokuClassifier has priority 100.
    /// </summary>
    [Fact]
    public void Priority_Is100()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(100, priority);
    }

    #endregion
}
