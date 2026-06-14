using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class TankaClassifierTests
{
    private readonly TankaClassifier _classifier = new();

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
    /// Verifies that a 5-7-5-7-7 syllable pattern is correctly classified as Tanka.
    /// </summary>
    [Fact]
    public void TryClassify_FiveSevenFiveSevenSeven_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "spring breeze carries scent", "cherry blossoms gently fall", "river gently flows", "children laugh along the bank", "memories drift with the stream" };
        var syllableCounts = new[] { 5, 7, 5, 7, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Tanka, definition!.Type);
        Assert.Equal(5, definition.LineCount);
        Assert.Equal([5, 7, 5, 7, 7], definition.SyllablesPerLine);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "spring breeze carries scent", "cherry blossoms gently fall", "river gently flows", "children laugh along the bank", "memories drift with the stream" };
        var syllableCounts = new[] { 5, 7, 5, 7, 5 };

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
        var lines = new[] { "spring breeze carries scent", "cherry blossoms gently fall", "river gently flows" };
        var syllableCounts = new[] { 5, 7, 5 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that TankaClassifier has priority 800.
    /// </summary>
    [Fact]
    public void Priority_Is800()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(800, priority);
    }

    #endregion
}
