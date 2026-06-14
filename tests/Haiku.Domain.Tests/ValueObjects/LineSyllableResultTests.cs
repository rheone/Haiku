using Haiku.Domain.ValueObjects;

namespace Haiku.Domain.Tests.ValueObjects;

public class LineSyllableResultTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var words = new[] { new SyllableResult("hello", 2, "CMU"), new SyllableResult("world", 1, "CMU") };

        var result = new LineSyllableResult(1, "hello world", 3, words);

        Assert.Equal(1, result.LineNumber);
        Assert.Equal("hello world", result.Text);
        Assert.Equal(3, result.TotalSyllables);
        Assert.Equal(2, result.Words.Length);
    }
}
