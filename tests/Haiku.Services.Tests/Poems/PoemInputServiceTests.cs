namespace Haiku.Services.Tests.Poems;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.PoemInputService"/> covering input
/// validation, normalization (trimming, line-ending conversion, zero-width character
/// removal), and syllable analysis of raw poem text.
/// </summary>
public class PoemInputServiceTests
{
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

    private readonly PoemInputService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="PoemInputServiceTests"/> class.
    /// Uses a real <see cref="SyllableEngine"/> with heuristic-only providers
    /// (no CMU dictionary file I/O) and a <see cref="PoemClassifierChain"/>
    /// with a mock classifier that always returns <see cref="PoemType.Haiku"/>.
    /// </summary>
    public PoemInputServiceTests()
    {
        var tokenizer = new WordTokenizer();
        var heuristic = new HeuristicSyllableProvider();
        var syllableEngine = new SyllableEngine([heuristic], tokenizer);

        var mockClassifier = Substitute.For<IPoemClassifier>();
        mockClassifier.Priority.Returns(100);
        mockClassifier
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = new PoemDefinition { Type = PoemType.Haiku };
                return true;
            });

        var classifierChain = new PoemClassifierChain([mockClassifier]);
        _service = new PoemInputService(syllableEngine, classifierChain, tokenizer);
    }

    #region Process

    [Fact]
    public void Process_EmptyInput_ReturnsInvalidWithError_Test()
    {
        // Act
        var result = _service.Process("");

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Poem is empty"));
    }

    [Fact]
    public void Process_WhitespaceInput_ReturnsInvalidWithError_Test()
    {
        // Act
        var result = _service.Process("   ");

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Poem is empty"));
    }

    [Fact]
    public void Process_ValidInput_SetsNormalizedContent_Test()
    {
        // Arrange
        var input = "hello world";

        // Act
        var result = _service.Process(input);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("hello world", result.NormalizedContent);
    }

    [Fact]
    public void Process_InputWithLeadingTrailingWhitespace_TrimsContent_Test()
    {
        // Arrange
        var input = "  hello world  ";

        // Act
        var result = _service.Process(input);

        // Assert
        Assert.Equal("hello world", result.NormalizedContent);
    }

    [Fact]
    public void Process_InputWithLineEndings_NormalizesToNewlines_Test()
    {
        // Arrange
        var input = "\r\nhello\r\nworld\r\n";

        // Act
        var result = _service.Process(input);

        // Assert
        Assert.Equal(2, result.Lines.Length);
        Assert.Equal("hello", result.Lines[0]);
        Assert.Equal("world", result.Lines[1]);
    }

    [Fact]
    public void Process_InputWithZeroWidthChars_RemovesThem_Test()
    {
        // Arrange
        var input = "hel\u200Blo\u200Cworld";

        // Act
        var result = _service.Process(input);

        // Assert
        Assert.Equal("helloworld", result.NormalizedContent);
    }

    #endregion
}
