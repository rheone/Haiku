using SyllableEngine = Haiku.Modules.Poems.Syllables.SyllableEngine;

namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>SyllableEngine</c> — verifying provider chain resolution and line syllable counting.
/// </summary>
public class SyllableEngineTests
{
    private static SyllableEngine CreateEngine(ISyllableProvider[]? providers = null, IWordTokenizer? tokenizer = null)
    {
        providers ??= [Substitute.For<ISyllableProvider>()];
        tokenizer ??= Substitute.For<IWordTokenizer>();
        return new SyllableEngine(providers, tokenizer);
    }

    #region CountWordSyllables

    /// <summary>
    ///     Verifies that the first provider to return a result wins and later providers are not consulted.
    /// </summary>
    [Fact]
    public void CountWordSyllables_FirstProviderWins_Test()
    {
        // Arrange
        var provider1 = Substitute.For<ISyllableProvider>();
        provider1
            .TryCountSyllables("test", out Arg.Any<SyllableResult?>())
            .Returns(x =>
            {
                x[1] = new SyllableResult("test", 1, "Custom");
                return true;
            });

        var provider2 = Substitute.For<ISyllableProvider>();
        var engine = CreateEngine([provider1, provider2]);

        // Act
        var result = engine.CountWordSyllables("test");

        // Assert
        Assert.Equal(1, result.Count);
        Assert.Equal("Custom", result.Tier);
    }

    /// <summary>
    ///     Verifies that when the first provider fails, the second provider is consulted.
    /// </summary>
    [Fact]
    public void CountWordSyllables_FallsbackToSecondProvider_Test()
    {
        // Arrange
        var provider1 = Substitute.For<ISyllableProvider>();
        provider1.TryCountSyllables("test", out Arg.Any<SyllableResult?>()).Returns(false);

        var provider2 = Substitute.For<ISyllableProvider>();
        provider2
            .TryCountSyllables("test", out Arg.Any<SyllableResult?>())
            .Returns(x =>
            {
                x[1] = new SyllableResult("test", 2, "CMU");
                return true;
            });

        var engine = CreateEngine([provider1, provider2]);

        // Act
        var result = engine.CountWordSyllables("test");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("CMU", result.Tier);
    }

    /// <summary>
    ///     Verifies that when no provider matches, a fallback result (tier "none") is returned.
    /// </summary>
    [Fact]
    public void CountWordSyllables_NoProviderMatches_ReturnsFallback_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);

        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("test");

        // Assert
        Assert.Equal(1, result.Count);
        Assert.Equal("none", result.Tier);
    }

    #endregion

    #region Special Token Normalization (SC-05, SC-06a, SC-06b)

    /// <summary>
    ///     Verifies that a numeral token is counted by expanding to its spoken English form
    ///     (SC-06a: "Numerals and numbers should be syllable-counted as their spoken word equivalent").
    /// </summary>
    [Fact]
    public void CountWordSyllables_Numeral_ExpandsToSpokenForm_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("42");

        // Assert
        // Humanizer: "42" → "forty-two" (spoken form). forty=2, two=1 → total 3.
        Assert.Equal(3, result.Count);
        Assert.Equal("Numeral", result.Tier);
    }

    /// <summary>
    ///     Verifies that a single-letter token uses the spoken letter name for syllable counting
    ///     (SC-05: "Individual letters should be syllable-counted as the number of syllables of the spoken letter").
    /// </summary>
    [Fact]
    public void CountWordSyllables_SingleLetter_UsesLetterName_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("A");

        // Assert
        // "A" = "AY" → 1 syllable.
        Assert.Equal(1, result.Count);
        Assert.Equal("LetterName", result.Tier);
    }

    /// <summary>
    ///     Verifies that a Roman numeral token is expanded to its spoken English form
    ///     (SC-06b: Roman numerals should be treated as numerals).
    /// </summary>
    [Fact]
    public void CountWordSyllables_RomanNumeral_ExpandsToSpokenForm_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("VII");

        // Assert
        // "VII" = 7 → "seven" → 2 syllables
        Assert.Equal(2, result.Count);
        Assert.Equal("RomanNumeral", result.Tier);
    }

    /// <summary>
    ///     Verifies that an ordinal token like "1st" is expanded via Humanizer's
    ///     ToOrdinalWords ("first") and counted accordingly.
    /// </summary>
    [Fact]
    public void CountWordSyllables_Ordinal_ExpandsToOrdinalWords_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("1st");

        // Assert
        // "1st" → "first" → 1 syllable (fir=1, st is part of "first" as one vowel group)
        Assert.Equal(1, result.Count);
        Assert.Equal("Ordinal", result.Tier);
    }

    /// <summary>
    ///     Verifies that "2nd" → "second" (2 syllables).
    /// </summary>
    [Fact]
    public void CountWordSyllables_OrdinalSecond_CountsTwo_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("2nd");

        // Assert
        // "2nd" → "second" → 2 syllables (se-cond)
        Assert.Equal(2, result.Count);
        Assert.Equal("Ordinal", result.Tier);
    }

    /// <summary>
    ///     Verifies that "101st" → "one hundred and first" (4 syllables).
    /// </summary>
    [Fact]
    public void CountWordSyllables_OrdinalLargeNumber_ExpandsCorrectly_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("101st");

        // Assert
        // "101st" → "one hundred first" (Humanizer omits "and") → one(1) + hun-dred(2) + first(1) = 4
        Assert.Equal(4, result.Count);
        Assert.Equal("Ordinal", result.Tier);
    }

    /// <summary>
    ///     Verifies that regular alphabetic words still go through the provider chain
    ///     and are not intercepted by special token normalization.
    /// </summary>
    [Fact]
    public void CountWordSyllables_RegularWord_StillUsesProviders_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider
            .TryCountSyllables("hello", out Arg.Any<SyllableResult?>())
            .Returns(x =>
            {
                x[1] = new SyllableResult("hello", 2, "CMU");
                return true;
            });
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("hello");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("CMU", result.Tier);
    }

    /// <summary>
    ///     Verifies that a negative numeral counts as the spoken word "negative" plus the numeral.
    /// </summary>
    [Fact]
    public void CountWordSyllables_NegativeNumeral_IncludesNegative_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("-5");

        // Assert
        // Humanizer: "-5" → "negative five" → negative=3, five=1 → total 4.
        Assert.Equal(4, result.Count);
        Assert.Equal("Numeral", result.Tier);
    }

    /// <summary>
    ///     Verifies that the numeral "0" produces a sensible syllable count.
    /// </summary>
    [Fact]
    public void CountWordSyllables_ZeroNumeral_ReturnsCount_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        provider.TryCountSyllables(Arg.Any<string>(), out Arg.Any<SyllableResult?>()).Returns(false);
        var engine = CreateEngine([provider]);

        // Act
        var result = engine.CountWordSyllables("0");

        // Assert
        // "0" → "zero" → 2 syllables
        Assert.Equal(2, result.Count);
        Assert.Equal("Numeral", result.Tier);
    }

    #endregion

    #region CountLineSyllables

    /// <summary>
    ///     Verifies that a non-empty line is tokenized and each word's syllables are summed.
    /// </summary>
    [Fact]
    public void CountLineSyllables_TokenizesAndCounts_Test()
    {
        // Arrange
        var provider1 = Substitute.For<ISyllableProvider>();
        provider1
            .TryCountSyllables("hello", out Arg.Any<SyllableResult?>())
            .Returns(x =>
            {
                x[1] = new SyllableResult("hello", 2, "CMU");
                return true;
            });
        provider1
            .TryCountSyllables("world", out Arg.Any<SyllableResult?>())
            .Returns(x =>
            {
                x[1] = new SyllableResult("world", 1, "CMU");
                return true;
            });

        var tokenizer = Substitute.For<IWordTokenizer>();
        tokenizer.Tokenize("hello world").Returns(new TokenizedLine { Words = ["hello", "world"], WordCount = 2 });

        var engine = CreateEngine([provider1], tokenizer);

        // Act
        var result = engine.CountLineSyllables("hello world");

        // Assert
        Assert.Equal(3, result.TotalSyllables);
        Assert.Equal(2, result.Words.Length);
    }

    /// <summary>
    ///     Verifies that an empty line returns a result with zero syllables and no words.
    /// </summary>
    [Fact]
    public void CountLineSyllables_EmptyLine_ReturnsEmpty_Test()
    {
        // Arrange
        var provider = Substitute.For<ISyllableProvider>();
        var tokenizer = Substitute.For<IWordTokenizer>();
        tokenizer.Tokenize("").Returns(new TokenizedLine());

        var engine = CreateEngine([provider], tokenizer);

        // Act
        var result = engine.CountLineSyllables("");

        // Assert
        Assert.Equal(0, result.TotalSyllables);
        Assert.Empty(result.Words);
    }

    #endregion
}
