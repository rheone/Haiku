using Haiku.Services.Rhyming.Providers;
using Haiku.Services.Syllables.Providers;

namespace Haiku.Services.Tests.Rhyming;

public class CmuRhymeProviderTests : IDisposable
{
    private readonly string _testDictPath;

    public CmuRhymeProviderTests()
    {
        _testDictPath = Path.GetTempFileName();
        File.WriteAllLines(_testDictPath, new[]
        {
            "night  N AY1 T",
            "light  L AY1 T",
            "day  D EY1",
            "moon  M UW1 N",
            "june  JH UW1 N",
            "hello  HH AH0 L OW1",
            "world  W ER1 L D",
            "xyz  K K K",
            "alpha  AA0 L F AH0",
        });
    }

    #region TryGetRhymeKey

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns the rhyme key for a known word with stress.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_KnownWordWithStress_ReturnsKey()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testDictPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        var result = provider.TryGetRhymeKey("night", out var key);

        // Assert
        Assert.True(result);
        Assert.Equal("AY T", key);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns false for an unknown word.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_UnknownWord_ReturnsFalse()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testDictPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        var result = provider.TryGetRhymeKey("unknownword", out var key);

        // Assert
        Assert.False(result);
        Assert.Null(key);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns the same key for words that rhyme.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_WordsWithSameRhyme_ReturnSameKey()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testDictPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        provider.TryGetRhymeKey("night", out var nightKey);
        provider.TryGetRhymeKey("light", out var lightKey);

        // Assert
        Assert.Equal(nightKey, lightKey);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns different keys for non-rhyming words.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_WordsWithDifferentRhyme_ReturnDifferentKeys()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testDictPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        provider.TryGetRhymeKey("moon", out var moonKey);
        provider.TryGetRhymeKey("day", out var dayKey);

        // Assert
        Assert.NotEqual(moonKey, dayKey);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey uses the last stressed syllable when only secondary stress exists.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_WordWithOnlySecondaryStress_UsesLastStressedSyllable()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testDictPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        var result = provider.TryGetRhymeKey("alpha", out var key);

        // Assert
        Assert.True(result);
        Assert.Equal("AH", key);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns false when the word has no digit in its phonemes.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_WordWithNoDigitInPhonemes_ReturnsNull()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testDictPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        var result = provider.TryGetRhymeKey("xyz", out var key);

        // Assert
        Assert.False(result);
        Assert.Null(key);
    }

    #endregion

    public void Dispose()
    {
        if (File.Exists(_testDictPath))
        {
            File.Delete(_testDictPath);
        }
    }
}
