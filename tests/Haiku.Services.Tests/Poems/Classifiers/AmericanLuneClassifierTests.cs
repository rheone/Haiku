namespace Haiku.Services.Tests.Poems.Classifiers;

public class AmericanLuneClassifierTests
{
    private readonly AmericanLuneClassifier _classifier = new();

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
    /// Verifies that a 3-5-3 syllable pattern is correctly classified as AmericanLune.
    /// </summary>
    [Fact]
    public void TryClassify_ThreeFiveThree_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "cold night air", "frost settles on the bare branch", "dawn approaches" };
        var syllableCounts = new[] { 3, 5, 3 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.AmericanLune, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "cold night air", "frost settles on the bare branch", "dawn approaches" };
        var syllableCounts = new[] { 3, 5, 5 };

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
        var lines = new[] { "cold night air", "frost settles on the bare branch" };
        var syllableCounts = new[] { 3, 5 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that AmericanLuneClassifier has priority 400.
    /// </summary>
    [Fact]
    public void Priority_Is400()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(400, priority);
    }

    #endregion
}
