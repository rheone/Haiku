using Haiku.Domain.Enums;
using Haiku.Services.Poems;

namespace Haiku.Tests;

/// <summary>Unit tests for the static detection and extraction methods in <see cref="PoemService"/>.</summary>
public class PoemServiceTests
{
    [Fact]
    public void DetectPoemType_ReturnsHaiku_For575()
    {
        var content = "line one\nline two\nline three";
        var counts = new List<int> { 5, 7, 5 };

        var result = PoemService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Haiku, result);
    }

    [Fact]
    public void DetectPoemType_ReturnsTanka_For57577()
    {
        var content = "1\n2\n3\n4\n5";
        var counts = new List<int> { 5, 7, 5, 7, 7 };

        var result = PoemService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Tanka, result);
    }

    [Fact]
    public void DetectPoemType_ReturnsMonoku_ForSingleLine()
    {
        var content = "A single line of text";
        var counts = new List<int> { 7 };

        var result = PoemService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public void DetectPoemType_ReturnsFreeform_WhenNoPatternMatches()
    {
        var content = "line one\nline two";
        var counts = new List<int> { 3, 9 };

        var result = PoemService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Freeform, result);
    }

    [Fact]
    public void ExtractTags_ReturnsDistinctLowercaseTags()
    {
        var content = "This is #Nature at its #best #Nature";

        var tags = PoemService.ExtractTags(content);

        Assert.Equal(2, tags.Count);
        Assert.Contains("nature", tags);
        Assert.Contains("best", tags);
    }

    [Fact]
    public void ExtractTags_ReturnsEmpty_WhenNoHashtags()
    {
        var content = "This has no tags at all";

        var tags = PoemService.ExtractTags(content);

        Assert.Empty(tags);
    }
}
