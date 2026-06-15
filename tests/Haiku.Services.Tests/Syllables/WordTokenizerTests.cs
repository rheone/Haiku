namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>WordTokenizer</c> — verifying text tokenization and punctuation stripping.
/// </summary>
public class WordTokenizerTests
{
    private readonly WordTokenizer _tokenizer = new();

    #region Tokenize

    /// <summary>
    ///     Verifies that a simple line is split into its constituent words.
    /// </summary>
    [Fact]
    public void Tokenize_SimpleLine_ReturnsWordsAndCounts_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("hello world");

        // Assert
        Assert.Equal(["hello", "world"], result.Words);
        Assert.Equal(2, result.WordCount);
    }

    /// <summary>
    ///     Verifies that an empty string returns an empty result.
    /// </summary>
    [Fact]
    public void Tokenize_EmptyLine_ReturnsEmpty_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("");

        // Assert
        Assert.Empty(result.Words);
        Assert.Equal(0, result.WordCount);
    }

    /// <summary>
    ///     Verifies that punctuation and ellipses are stripped from tokens.
    /// </summary>
    [Fact]
    public void Tokenize_Punctuation_StripsNonSpokenChars_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("hello, world! foo...");

        // Assert
        Assert.Equal(["hello", "world", "foo"], result.Words);
        Assert.Equal(3, result.WordCount);
    }

    /// <summary>
    ///     Verifies that emoji characters are excluded from tokens.
    /// </summary>
    [Fact]
    public void Tokenize_Emoji_ExcludesEmoji_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("hello 🌸 world");

        // Assert
        Assert.Equal(["hello", "world"], result.Words);
    }

    /// <summary>
    ///     Verifies that numeric digits are kept as words.
    /// </summary>
    [Fact]
    public void Tokenize_Numerals_KeepsDigitsAsWords_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("line 2 of 3");

        // Assert
        Assert.Equal(["line", "2", "of", "3"], result.Words);
        Assert.Equal(4, result.WordCount);
    }

    /// <summary>
    ///     Verifies that uppercase Roman numerals are detected and kept as words.
    /// </summary>
    [Fact]
    public void Tokenize_RomanNumerals_DetectsUppercase_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("Chapter VII");

        // Assert
        Assert.Equal(["Chapter", "VII"], result.Words);
    }

    /// <summary>
    ///     Verifies that hyphens are stripped from hyphenated words.
    /// </summary>
    [Fact]
    public void Tokenize_HyphenatedWord_StripsHyphen_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("well-known");

        // Assert
        Assert.Equal(["wellknown"], result.Words);
    }

    /// <summary>
    ///     Verifies that ordinal tokens like "1st", "2nd", "3rd" are kept as words.
    /// </summary>
    [Fact]
    public void Tokenize_OrdinalTokens_KeptAsWords_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("my 1st haiku on 2nd try");

        // Assert
        Assert.Equal(["my", "1st", "haiku", "on", "2nd", "try"], result.Words);
        Assert.Equal(6, result.WordCount);
    }

    /// <summary>
    ///     Verifies that "th" suffixed ordinals and larger numbers work.
    /// </summary>
    [Fact]
    public void Tokenize_OrdinalTokens_LargerNumbers_Test()
    {
        // Arrange (implicit — no setup beyond the field initializer)

        // Act
        var result = _tokenizer.Tokenize("the 42nd and 101st place");

        // Assert
        Assert.Equal(["the", "42nd", "and", "101st", "place"], result.Words);
    }

    #endregion
}
