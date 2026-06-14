using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class KellyLuneClassifierTests
{
    private readonly KellyLuneClassifier _classifier = new();

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
    /// Verifies that a 5-3-5 syllable pattern is correctly classified as KellyLune.
    /// </summary>
    [Fact]
    public void TryClassify_FiveThreeFive_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "rain upon the roof", "soft tapping", "lulling me to peaceful sleep" };
        var syllableCounts = new[] { 5, 3, 5 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.KellyLune, definition!.Type);
        Assert.Equal(3, definition.LineCount);
        Assert.Equal([5, 3, 5], definition.SyllablesPerLine);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "rain upon the roof", "soft tapping", "lulling me to peaceful sleep" };
        var syllableCounts = new[] { 5, 5, 5 };

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
        var lines = new[] { "rain upon the roof", "soft tapping" };
        var syllableCounts = new[] { 5, 3 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that KellyLuneClassifier has priority 500.
    /// </summary>
    [Fact]
    public void Priority_Is500()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(500, priority);
    }

    #endregion
}
