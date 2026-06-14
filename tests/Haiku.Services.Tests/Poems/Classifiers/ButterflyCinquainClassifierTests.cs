using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class ButterflyCinquainClassifierTests
{
    private readonly ButterflyCinquainClassifier _classifier = new();

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
    /// Verifies that a 2-4-6-8-2-8-6-4-2 syllable pattern is correctly classified as ButterflyCinquain.
    /// </summary>
    [Fact]
    public void TryClassify_CorrectPattern_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "soft breeze", "whispers through the tall pine trees", "gentle rustling sounds", "crickets chirping in the evening", "peace falls", "sunlight fades across the hills", "shadows stretch and grow", "stars appear one by one", "night descends" };
        var syllableCounts = new[] { 2, 4, 6, 8, 2, 8, 6, 4, 2 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.ButterflyCinquain, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "soft breeze", "whispers through the tall pine trees", "gentle rustling sounds", "crickets chirping in the evening", "peace falls", "sunlight fades across the hills", "shadows stretch and grow", "stars appear one by one", "wrong here" };
        var syllableCounts = new[] { 2, 4, 6, 8, 2, 8, 6, 4, 3 };

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
        var lines = new[] { "soft breeze", "whispers through the tall pine trees", "gentle rustling sounds", "crickets chirping in the evening" };
        var syllableCounts = new[] { 2, 4, 6, 8 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that ButterflyCinquainClassifier has priority 1200.
    /// </summary>
    [Fact]
    public void Priority_Is1200()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(1200, priority);
    }

    #endregion
}
