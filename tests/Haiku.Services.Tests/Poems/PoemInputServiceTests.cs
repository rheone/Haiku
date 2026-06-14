namespace Haiku.Services.Tests.Poems;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.PoemInputService"/> covering input
/// validation, normalization (trimming, line-ending conversion, zero-width character
/// removal), and syllable analysis of raw poem text.
/// </summary>
public class PoemInputServiceTests
{
    private readonly SyllableEngine _syllableEngine;
    private readonly IPoemMatcherChain _matcherChain;
    private readonly PoemInputService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="PoemInputServiceTests"/> class.
    /// Creates a partial substitute of <see cref="SyllableEngine"/> with empty
    /// dictionary and CMU word sets, and a mock <see cref="IPoemMatcherChain"/>
    /// that defaults to returning <see cref="PoemType.Haiku"/>.
    /// </summary>
    public PoemInputServiceTests()
    {
        // Partial substitute: allows real method execution while mocking abstract
        // base behavior. SyllableEngine is a singleton that loads dictionary.dic;
        // providing empty collections avoids file I/O in unit tests.
        _syllableEngine = Substitute.For<SyllableEngine>(new Dictionary<string, int>(), new HashSet<string>());
        _matcherChain = Substitute.For<IPoemMatcherChain>();
        _matcherChain.Match(Arg.Any<string[]>(), Arg.Any<int[]>()).Returns(PoemType.Haiku);
        _service = new PoemInputService(_syllableEngine, _matcherChain);
    }

    [Fact]
    public void Process_EmptyInput_ReturnsInvalidWithError()
    {
        var result = _service.Process("");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Poem is empty"));
    }

    [Fact]
    public void Process_WhitespaceInput_ReturnsInvalidWithError()
    {
        var result = _service.Process("   ");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Poem is empty"));
    }

    [Fact]
    public void Process_ValidInput_SetsNormalizedContent()
    {
        _syllableEngine.CountLineSyllables(Arg.Any<string>()).Returns(new List<int> { 1, 1 });

        var result = _service.Process("hello world");

        Assert.True(result.IsValid);
        Assert.Equal("hello world", result.NormalizedContent);
    }

    [Fact]
    public void Process_InputWithLeadingTrailingWhitespace_TrimsContent()
    {
        _syllableEngine.CountLineSyllables(Arg.Any<string>()).Returns(new List<int> { 2 });

        var result = _service.Process("  hello world  ");

        Assert.Equal("hello world", result.NormalizedContent);
    }

    [Fact]
    public void Process_InputWithLineEndings_NormalizesToNewlines()
    {
        _syllableEngine.CountLineSyllables(Arg.Any<string>()).Returns(new List<int> { 1 });

        var result = _service.Process("\r\nhello\r\nworld\r\n");

        Assert.Equal(2, result.Lines.Length);
        Assert.Equal("hello", result.Lines[0]);
        Assert.Equal("world", result.Lines[1]);
    }

    [Fact]
    public void Process_InputWithZeroWidthChars_RemovesThem()
    {
        _syllableEngine.CountLineSyllables(Arg.Any<string>()).Returns(new List<int> { 1 });

        var result = _service.Process("hel\u200Blo\u200Cworld");

        Assert.Equal("helloworld", result.NormalizedContent);
    }
}
