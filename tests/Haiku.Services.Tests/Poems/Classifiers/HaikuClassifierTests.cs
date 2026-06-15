namespace Haiku.Services.Tests.Poems.Classifiers;

public class HaikuClassifierTests
{
    private readonly HaikuClassifier _classifier = new();

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
    /// Verifies that a 5-7-5 syllable pattern is correctly classified as Haiku.
    /// </summary>
    [Fact]
    public void TryClassify_FiveSevenFive_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "an old silent pond", "a frog jumps into the pond", "splash silence again" };
        var syllableCounts = new[] { 5, 7, 5 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Haiku, definition!.Type);
    }

    /// <summary>
    /// Verifies that a non-matching syllable pattern returns false.
    /// </summary>
    [Fact]
    public void TryClassify_WrongPattern_ReturnsFalse()
    {
        // Arrange
        var lines = new[] { "hello world", "foo bar baz", "hi" };
        var syllableCounts = new[] { 2, 3, 1 };

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
        var lines = new[] { "line one", "line two" };
        var syllableCounts = new[] { 5, 7 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that HaikuClassifier has priority 200.
    /// </summary>
    [Fact]
    public void Priority_Is200()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(200, priority);
    }

    #endregion
}
