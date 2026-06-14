using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class CompressedClassifierTests
{
    private readonly CompressedClassifier _classifier = new();

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
    /// Verifies that a 2-3-2 syllable pattern is correctly classified as Compressed.
    /// </summary>
    [Fact]
    public void TryClassify_TwoThreeTwo_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "deep snow", "cold wind blows through the night", "warm fire" };
        var syllableCounts = new[] { 2, 3, 2 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Compressed, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "deep snow", "cold wind blows through the night", "warm fire" };
        var syllableCounts = new[] { 2, 3, 3 };

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
        var lines = new[] { "deep snow", "cold wind blows through the night" };
        var syllableCounts = new[] { 2, 3 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that CompressedClassifier has priority 600.
    /// </summary>
    [Fact]
    public void Priority_Is600()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(600, priority);
    }

    #endregion
}
