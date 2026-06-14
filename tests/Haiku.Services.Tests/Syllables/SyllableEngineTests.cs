using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;
using SyllableEngine = Haiku.Services.Syllables.SyllableEngine;

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
    public void CountWordSyllables_FirstProviderWins()
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
    public void CountWordSyllables_FallsbackToSecondProvider()
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
    public void CountWordSyllables_NoProviderMatches_ReturnsFallback()
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

    #region CountLineSyllables

    /// <summary>
    ///     Verifies that a non-empty line is tokenized and each word's syllables are summed.
    /// </summary>
    [Fact]
    public void CountLineSyllables_TokenizesAndCounts()
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
    public void CountLineSyllables_EmptyLine_ReturnsEmpty()
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
