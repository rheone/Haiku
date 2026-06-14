using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class AmericanCinquainClassifierTests
{
    private readonly AmericanCinquainClassifier _classifier = new();

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
    /// Verifies that a 2-4-6-8-2 syllable pattern is correctly classified as AmericanCinquain.
    /// </summary>
    [Fact]
    public void TryClassify_TwoFourSixEightTwo_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "shy moon", "soft light upon still pond", "rippling silver gleams", "frogs leap into the cool night air", "splash heard" };
        var syllableCounts = new[] { 2, 4, 6, 8, 2 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.AmericanCinquain, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "shy moon", "soft light upon still pond", "rippling silver gleams", "frogs leap into the cool night air", "wrong here" };
        var syllableCounts = new[] { 2, 4, 6, 8, 3 };

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
        var lines = new[] { "shy moon", "soft light upon still pond", "rippling silver gleams" };
        var syllableCounts = new[] { 2, 4, 6 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that AmericanCinquainClassifier has priority 900.
    /// </summary>
    [Fact]
    public void Priority_Is900()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(900, priority);
    }

    #endregion
}
