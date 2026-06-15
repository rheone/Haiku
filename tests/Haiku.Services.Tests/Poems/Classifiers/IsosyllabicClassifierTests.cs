namespace Haiku.Services.Tests.Poems.Classifiers;

public class IsosyllabicClassifierTests
{
    private readonly IsosyllabicClassifier _classifier = new();

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
    /// Verifies that a uniform 7-7-7 syllable pattern is correctly classified as Isosyllabic.
    /// </summary>
    [Fact]
    public void TryClassify_AllSeven_ReturnsDefinition_Test()
    {
        // Arrange
        var lines = new[]
        {
            "the moon rises high tonight",
            "stars twinkle across the dark sky",
            "night birds sing their gentle songs",
        };
        var syllableCounts = new[] { 7, 7, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Isosyllabic, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-uniform syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse_Test()
    {
        // Arrange
        var lines = new[] { "the moon rises high tonight", "stars in the sky", "night birds sing their gentle songs" };
        var syllableCounts = new[] { 7, 5, 7 };

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
        var lines = new[] { "the moon rises high tonight" };
        var syllableCounts = new[] { 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that IsosyllabicClassifier has priority 1500.
    /// </summary>
    [Fact]
    public void Priority_Is1500_Test()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(1500, priority);
    }

    #endregion
}
