using Haiku.Services.Poems;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="PoemService"/>.</summary>
public class PoemServiceTests
{
    #region ExtractTags

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> returns distinct lowercase tags.
    /// </summary>
    [Fact]
    public void ExtractTags_ReturnsDistinctLowercaseTags()
    {
        // Arrange
        var content = "This is #Nature at its #best #Nature";

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Equal(2, tags.Count);
        Assert.Contains("nature", tags);
        Assert.Contains("best", tags);
    }

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> returns an empty list when no hashtags are present.
    /// </summary>
    [Fact]
    public void ExtractTags_ReturnsEmpty_WhenNoHashtags()
    {
        // Arrange
        var content = "This has no tags at all";

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Empty(tags);
    }

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> throws when content is null.
    /// </summary>
    [Fact]
    public void ExtractTags_NullContent_ThrowsArgumentNullException_Test()
    {
        // Auto Generated, verify expected behavior:
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => PoemService.ExtractTags(null!));
    }

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> returns an empty list for empty string content.
    /// </summary>
    [Fact]
    public void ExtractTags_EmptyString_ReturnsEmpty_Test()
    {
        // Auto Generated, verify expected behavior:
        // Arrange
        var content = string.Empty;

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Empty(tags);
    }

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> returns lowercase tags when input has mixed case.
    /// </summary>
    [Fact]
    public void ExtractTags_MixedCase_ReturnsLowercase_Test()
    {
        // Auto Generated, verify expected behavior:
        // Arrange
        var content = "#Nature #BEAUTY #Science";

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Equal(3, tags.Count);
        Assert.Contains("nature", tags);
        Assert.Contains("beauty", tags);
        Assert.Contains("science", tags);
    }

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> returns an empty list when content contains only '#' symbols.
    /// </summary>
    [Fact]
    public void ExtractTags_OnlyHashSymbol_ReturnsEmpty_Test()
    {
        // Auto Generated, verify expected behavior:
        // Arrange
        var content = "# # #";

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Empty(tags);
    }

    /// <summary>
    /// Tests that <see cref="PoemService.ExtractTags"/> handles consecutive hashtags correctly.
    /// </summary>
    [Fact]
    public void ExtractTags_ConsecutiveHashtags_ReturnsTags_Test()
    {
        // Auto Generated, verify expected behavior:
        // Arrange
        var content = "##urgent ##important";

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Equal(2, tags.Count);
        Assert.Contains("urgent", tags);
        Assert.Contains("important", tags);
    }

    #endregion
}
