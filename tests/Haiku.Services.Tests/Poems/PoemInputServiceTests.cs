using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems;

public class PoemInputServiceTests
{
    private readonly IWordTokenizer _tokenizer;
    private readonly PoemInputService _service;

    public PoemInputServiceTests()
    {
        _tokenizer = Substitute.For<IWordTokenizer>();

        var provider = Substitute.For<ISyllableProvider>();
        provider
            .TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>())
            .Returns(x =>
            {
                x[1] = new SyllableResult("test", 1, "Heuristic");
                return true;
            });

        var syllableEngine = new global::Haiku.Services.Syllables.SyllableEngine([provider], Substitute.For<IWordTokenizer>());

        var classifier = Substitute.For<IPoemClassifier>();
        classifier.Priority.Returns(1);
        classifier
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = new PoemDefinition { Type = PoemType.Haiku };
                return true;
            });

        var classifierChain = new PoemClassifierChain([classifier]);
        _service = new PoemInputService(syllableEngine, classifierChain, _tokenizer);
    }

    #region Process

    /// <summary>
    ///     Verifies that Process returns invalid with an error when input is empty.
    /// </summary>
    [Fact]
    public void Process_EmptyInput_ReturnsInvalidWithError()
    {
        // Arrange
        var result = _service.Process("");

        // Act

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Poem is empty"));
    }

    /// <summary>
    ///     Verifies that Process returns invalid with an error when input is whitespace-only.
    /// </summary>
    [Fact]
    public void Process_WhitespaceInput_ReturnsInvalidWithError()
    {
        // Arrange
        var result = _service.Process("   ");

        // Act

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Poem is empty"));
    }

    /// <summary>
    ///     Verifies that Process sets normalized content for valid input.
    /// </summary>
    [Fact]
    public void Process_ValidInput_SetsNormalizedContent()
    {
        // Arrange
        _tokenizer.Tokenize(Arg.Any<string>()).Returns(new TokenizedLine { Words = ["hello", "world"], WordCount = 2 });

        // Act
        var result = _service.Process("hello world");

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("hello world", result.NormalizedContent);
    }

    /// <summary>
    ///     Verifies that Process trims leading and trailing whitespace from input.
    /// </summary>
    [Fact]
    public void Process_InputWithLeadingTrailingWhitespace_TrimsContent()
    {
        // Arrange
        _tokenizer.Tokenize(Arg.Any<string>()).Returns(new TokenizedLine { Words = ["hello", "world"], WordCount = 2 });

        // Act
        var result = _service.Process("  hello world  ");

        // Assert
        Assert.Equal("hello world", result.NormalizedContent);
    }

    /// <summary>
    ///     Verifies that Process normalizes line endings to newlines.
    /// </summary>
    [Fact]
    public void Process_InputWithLineEndings_NormalizesToNewlines()
    {
        // Arrange
        _tokenizer.Tokenize(Arg.Any<string>()).Returns(new TokenizedLine { Words = ["hello"], WordCount = 1 });

        // Act
        var result = _service.Process("\r\nhello\r\nworld\r\n");

        // Assert
        Assert.Equal(2, result.Lines.Length);
        Assert.Equal("hello", result.Lines[0]);
        Assert.Equal("world", result.Lines[1]);
    }

    /// <summary>
    ///     Verifies that Process removes zero-width characters from input.
    /// </summary>
    [Fact]
    public void Process_InputWithZeroWidthChars_RemovesThem()
    {
        // Arrange
        _tokenizer.Tokenize(Arg.Any<string>()).Returns(new TokenizedLine { Words = ["helloworld"], WordCount = 1 });

        // Act
        var result = _service.Process("hel\u200Blo\u200Cworld");

        // Assert
        Assert.Equal("helloworld", result.NormalizedContent);
    }

    #endregion
}
