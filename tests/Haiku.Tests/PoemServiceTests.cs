namespace Haiku.Tests;

/// <summary>Unit tests for the static detection and extraction methods in <see cref="PoemService"/>.</summary>
/// <remarks>
/// <para>
/// Poem type detection relies on the <c>PoemMatcherChain</c> — a priority-ordered chain of
/// matchers that test syllable-count patterns against known poetic forms (haiku, tanka,
/// monoku, etc.). The chain returns the first match, so matcher order matters. See
/// <see cref="PoemEngine"/> for the full set of supported forms.
/// </para>
/// </remarks>
public class PoemServiceTests
{
    #region DetectPoemType

    /// <summary>Verifies that a 5-7-5 syllable pattern on three lines is correctly detected as a Haiku.</summary>
    [Fact]
    public void DetectPoemType_ReturnsHaiku_For575_Test()
    {
        // Arrange
        var content = "line one\nline two\nline three";
        var counts = new List<int> { 5, 7, 5 };

        // Act
        var result = PoemService.DetectPoemType(content, counts);

        // Assert
        Assert.Equal(PoemType.Haiku, result);
    }

    /// <summary>Verifies that a 5-7-5-7-7 syllable pattern on five lines is correctly detected as a Tanka.</summary>
    [Fact]
    public void DetectPoemType_ReturnsTanka_For57577_Test()
    {
        // Arrange
        var content = "1\n2\n3\n4\n5";
        var counts = new List<int> { 5, 7, 5, 7, 7 };

        // Act
        var result = PoemService.DetectPoemType(content, counts);

        // Assert
        Assert.Equal(PoemType.Tanka, result);
    }

    /// <summary>Verifies that a single line with a 7-syllable count is correctly detected as a Monoku.</summary>
    [Fact]
    public void DetectPoemType_ReturnsMonoku_ForSingleLine_Test()
    {
        // Arrange
        var content = "A single line of text";
        var counts = new List<int> { 7 };

        // Act
        var result = PoemService.DetectPoemType(content, counts);

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    /// <summary>Verifies that an unrecognized syllable-count pattern falls back to Freeform.</summary>
    [Fact]
    public void DetectPoemType_ReturnsFreeform_WhenNoPatternMatches_Test()
    {
        // Arrange
        var content = "line one\nline two";
        var counts = new List<int> { 3, 9 };

        // Act
        var result = PoemService.DetectPoemType(content, counts);

        // Assert
        Assert.Equal(PoemType.Freeform, result);
    }

    #endregion

    #region ExtractTags

    /// <summary>Verifies that hashtags are extracted in distinct lowercase form, ignoring duplicates.</summary>
    [Fact]
    public void ExtractTags_ReturnsDistinctLowercaseTags_Test()
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

    /// <summary>Verifies that content without any hashtags returns an empty list.</summary>
    [Fact]
    public void ExtractTags_ReturnsEmpty_WhenNoHashtags_Test()
    {
        // Arrange
        var content = "This has no tags at all";

        // Act
        var tags = PoemService.ExtractTags(content);

        // Assert
        Assert.Empty(tags);
    }

    #endregion
}
