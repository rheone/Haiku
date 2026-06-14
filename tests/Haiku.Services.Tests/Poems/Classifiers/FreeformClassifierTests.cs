using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class FreeformClassifierTests
{
    private readonly FreeformClassifier _classifier = new();

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
    /// Verifies that any input is correctly classified as Freeform.
    /// </summary>
    [Fact]
    public void TryClassify_AnyInput_ReturnsDefinition()
    {
        // Arrange
        var lines = new[] { "any words", "here they come", "more lines of text" };
        var syllableCounts = new[] { 3, 4, 4 };

        // Act
        var success = _classifier.TryClassify(lines, syllableCounts, Tokenize(lines), out var definition);

        // Assert
        Assert.True(success);
        Assert.NotNull(definition);
        Assert.Equal(PoemType.Freeform, definition!.Type);
        Assert.Equal(3, definition.LineCount);
    }

    #endregion

    #region Priority

    /// <summary>
    /// Verifies that FreeformClassifier has the lowest priority (int.MaxValue).
    /// </summary>
    [Fact]
    public void Priority_IsMaxValue()
    {
        // Arrange

        // Act
        var priority = _classifier.Priority;

        // Assert
        Assert.Equal(int.MaxValue, priority);
    }

    #endregion
}
