namespace Haiku.Services.Tests.Poems.Classifiers;

public class NearTraditionalClassifierTests
{
    private readonly NearTraditionalClassifier _classifier = new();

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
    /// Verifies that a 4-6-4 syllable pattern is correctly classified as NearTraditional.
    /// </summary>
    [Fact]
    public void TryClassify_FourSixFour_ReturnsDefinition_Test()
    {
        // Arrange
        var lines = new[] { "falling leaves", "dance upon the autumn breeze", "golden glow" };
        var syllableCounts = new[] { 4, 6, 4 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.NearTraditional, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse_Test()
    {
        // Arrange
        var lines = new[] { "falling leaves", "dance upon the autumn breeze", "golden glow" };
        var syllableCounts = new[] { 4, 6, 5 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    /// <summary>
    /// Verifies that an incorrect line count returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongLineCount_ReturnsFalse_Test()
    {
        // Arrange
        var lines = new[] { "falling leaves", "dance upon the autumn breeze" };
        var syllableCounts = new[] { 4, 6 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that NearTraditionalClassifier has priority 700.
    /// </summary>
    [Fact]
    public void Priority_Is700_Test()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(700, priority);
    }

    #endregion
}
