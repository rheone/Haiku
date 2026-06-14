using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class ReverseCinquainClassifierTests
{
    private readonly ReverseCinquainClassifier _classifier = new();

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
    /// Verifies that a 2-8-6-4-2 syllable pattern is correctly classified as ReverseCinquain.
    /// </summary>
    [Fact]
    public void TryClassify_TwoEightSixFourTwo_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "night breeze", "moonbeams dance upon the quiet lake", "silver ripples spread", "trees whisper in the dark", "peace descends" };
        var syllableCounts = new[] { 2, 8, 6, 4, 2 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.ReverseCinquain, definition!.Type);
        Assert.Equal(5, definition.LineCount);
        Assert.Equal([2, 8, 6, 4, 2], definition.SyllablesPerLine);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "night breeze", "moonbeams dance upon the quiet lake", "silver ripples spread", "trees whisper in the dark", "peace descends" };
        var syllableCounts = new[] { 2, 8, 6, 4, 3 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    /// <summary>
    /// Verifies that an incorrect line count returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongLineCount_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "night breeze", "moonbeams dance upon the quiet lake", "silver ripples spread" };
        var syllableCounts = new[] { 2, 8, 6 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that ReverseCinquainClassifier has priority 1000.
    /// </summary>
    [Fact]
    public void Priority_Is1000()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(1000, priority);
    }

    #endregion
}
