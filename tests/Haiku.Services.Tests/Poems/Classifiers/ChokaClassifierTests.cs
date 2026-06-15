namespace Haiku.Services.Tests.Poems.Classifiers;

public class ChokaClassifierTests
{
    private readonly ChokaClassifier _classifier = new();

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
    /// Verifies that alternating 5-7-5-7-5-7-7 syllable pattern is correctly classified as Choka.
    /// </summary>
    [Fact]
    public void TryClassify_SevenLinesAlternating_ReturnsDefinition_Test()
    {
        // Arrange
        var lines = new[]
        {
            "autumn wind blows",
            "leaves scatter across the yard",
            "crisp chill in the air",
            "children play in golden piles",
            "flaming colors glow",
            "harvest moon shines full tonight",
            "geese call out as they fly south",
        };
        var syllableCounts = new[] { 5, 7, 5, 7, 5, 7, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Choka, definition!.Type);
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
            "autumn wind blows",
            "leaves scatter across the yard",
            "crisp chill in the air",
            "children play in golden piles",
            "flaming colors glow",
            "harvest moon shines full tonight",
            "geese call out as they fly south",
        };
        var syllableCounts = new[] { 5, 7, 5, 7, 5, 7, 5 };

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
            "autumn wind blows",
            "leaves scatter across the yard",
            "crisp chill in the air",
            "children play in golden piles",
            "flaming colors glow",
            "harvest moon shines",
        };
        var syllableCounts = new[] { 5, 7, 5, 7, 5, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that ChokaClassifier has priority 1400.
    /// </summary>
    [Fact]
    public void Priority_Is1400_Test()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(1400, priority);
    }

    #endregion
}
