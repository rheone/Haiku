using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;

namespace Haiku.Domain.Tests.ValueObjects;

public class PoemDefinitionTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var definition = new PoemDefinition
        {
            Type = PoemType.Haiku,
            LineCount = 3,
            SyllablesPerLine = [5, 7, 5],
            TotalSyllableCount = 17,
            WordCountPerLine = [2, 3, 2],
            TotalWordCount = 7,
            OriginalContent = "hello world\nfoo bar baz\nhi there",
            NormalizedContent = "hello world foo bar baz hi there",
            Theme = "nature",
        };

        Assert.Equal(PoemType.Haiku, definition.Type);
        Assert.Equal(3, definition.LineCount);
        Assert.Equal([5, 7, 5], definition.SyllablesPerLine);
        Assert.Equal(17, definition.TotalSyllableCount);
        Assert.Equal([2, 3, 2], definition.WordCountPerLine);
        Assert.Equal(7, definition.TotalWordCount);
        Assert.Equal("hello world\nfoo bar baz\nhi there", definition.OriginalContent);
        Assert.Equal("hello world foo bar baz hi there", definition.NormalizedContent);
        Assert.Equal("nature", definition.Theme);
    }

    [Fact]
    public void Theme_CanBeNull()
    {
        var definition = new PoemDefinition { Type = PoemType.Freeform };

        Assert.Null(definition.Theme);
    }

    [Fact]
    public void Metadata_CanBeNull()
    {
        var definition = new PoemDefinition { Type = PoemType.Freeform };

        Assert.Null(definition.Metadata);
    }
}
