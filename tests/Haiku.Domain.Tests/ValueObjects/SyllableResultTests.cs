using Haiku.Domain.ValueObjects;

namespace Haiku.Domain.Tests.ValueObjects;

public class SyllableResultTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var result = new SyllableResult("hello", 2, "CMU");

        Assert.Equal("hello", result.Word);
        Assert.Equal(2, result.Count);
        Assert.Equal("CMU", result.Tier);
    }
}
