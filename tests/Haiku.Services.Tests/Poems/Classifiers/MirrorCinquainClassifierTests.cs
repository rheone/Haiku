namespace Haiku.Services.Tests.Poems.Classifiers;

public class MirrorCinquainClassifierTests
{
    private readonly MirrorCinquainClassifier _classifier = new();

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
    /// Verifies that a 2-4-6-8-2-2-8-6-4-2 syllable pattern is correctly classified as MirrorCinquain.
    /// </summary>
    [Fact]
    public void TryClassify_CorrectPattern_ReturnsDefinition_Test()
    {
        // Arrange
        var lines = new[]
        {
            "dawn breaks",
            "golden light spreads across hills",
            "morning mist rising",
            "birds begin their cheerful songs",
            "new day",
            "fresh start",
            "sun warms the cooling evening air",
            "shadows creeping long",
            "stars appear in darkening sky",
            "night falls",
        };
        var syllableCounts = new[] { 2, 4, 6, 8, 2, 2, 8, 6, 4, 2 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.MirrorCinquain, definition!.Type);
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
            "dawn breaks",
            "golden light spreads across hills",
            "morning mist rising",
            "birds begin their cheerful songs",
            "new day",
            "fresh start",
            "sun warms the cooling evening air",
            "shadows creeping long",
            "stars appear in darkening sky",
            "wrong word",
        };
        var syllableCounts = new[] { 2, 4, 6, 8, 2, 2, 8, 6, 4, 3 };

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
        var lines = new[]
        {
            "dawn breaks",
            "golden light spreads across hills",
            "morning mist rising",
            "birds begin their cheerful songs",
            "new day",
        };
        var syllableCounts = new[] { 2, 4, 6, 8, 2 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that MirrorCinquainClassifier has priority 1300.
    /// </summary>
    [Fact]
    public void Priority_Is1300_Test()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(1300, priority);
    }

    #endregion
}
