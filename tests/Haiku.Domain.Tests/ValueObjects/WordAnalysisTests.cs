namespace Haiku.Domain.Tests.ValueObjects;

public class WordAnalysisTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var analysis = new WordAnalysis
        {
            Word = "hello",
            Syllables = 2,
            Tier = "CMU",
        };

        Assert.Equal("hello", analysis.Word);
        Assert.Equal(2, analysis.Syllables);
        Assert.Equal("CMU", analysis.Tier);
    }
}
