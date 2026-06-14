using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class KatautaClassifierTests
{
    private readonly KatautaClassifier _classifier = new();

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
    /// Verifies that a 5-7-7 syllable pattern is correctly classified as Katauta.
    /// </summary>
    [Fact]
    public void TryClassify_FiveSevenSeven_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "cherry blossoms fall", "petals dance upon the soft breeze", "spring whispers goodbye once more" };
        var syllableCounts = new[] { 5, 7, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Katauta, definition!.Type);
        Assert.Equal(3, definition.LineCount);
        Assert.Equal([5, 7, 7], definition.SyllablesPerLine);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "cherry blossoms fall", "petals dance upon the soft breeze", "spring whispers goodbye" };
        var syllableCounts = new[] { 5, 7, 5 };

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
        var lines = new[] { "cherry blossoms fall", "petals dance upon the soft breeze" };
        var syllableCounts = new[] { 5, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that KatautaClassifier has priority 300.
    /// </summary>
    [Fact]
    public void Priority_Is300()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(300, priority);
    }

    #endregion
}
