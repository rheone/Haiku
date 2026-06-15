namespace Haiku.Services.Tests.Poems.Classifiers;

public class SedokaClassifierTests
{
    private readonly SedokaClassifier _classifier = new();

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
    /// Verifies that a 5-7-7-5-7-7 syllable pattern is correctly classified as Sedoka.
    /// </summary>
    [Fact]
    public void TryClassify_CorrectPattern_ReturnsDefinition_Test()
    {
        // Arrange
        var lines = new[]
        {
            "morning sun rises",
            "birds begin their joyful song",
            "dew drops glisten on green leaves",
            "evening shadows fall",
            "crickets start their nightly choir",
            "stars appear one by one above",
        };
        var syllableCounts = new[] { 5, 7, 7, 5, 7, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Sedoka, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse_Test()
    {
        // Arrange
        var lines = new[]
        {
            "morning sun rises",
            "birds begin their joyful song",
            "dew drops glisten on green leaves",
            "evening shadows fall",
            "crickets start their nightly choir",
            "stars appear one by one",
        };
        var syllableCounts = new[] { 5, 7, 7, 5, 7, 5 };

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
        var lines = new[] { "morning sun rises", "birds begin their joyful song", "dew drops glisten on green leaves" };
        var syllableCounts = new[] { 5, 7, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that SedokaClassifier has priority 1100.
    /// </summary>
    [Fact]
    public void Priority_Is1100_Test()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(1100, priority);
    }

    #endregion
}
