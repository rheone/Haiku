namespace Haiku.Services.Tests.Rhyming;

public sealed class CmuRhymeProviderTests : IDisposable
{
    private readonly string _testJsonPath;

    public CmuRhymeProviderTests()
    {
        _testJsonPath = Path.GetTempFileName();
        var json = """
            {
              "entries": {
                "night": [{ "s": 1, "p": ["N", "AY1", "T"] }],
                "light": [{ "s": 1, "p": ["L", "AY1", "T"] }],
                "day": [{ "s": 1, "p": ["D", "EY1"] }],
                "moon": [{ "s": 1, "p": ["M", "UW1", "N"] }],
                "june": [{ "s": 1, "p": ["JH", "UW1", "N"] }],
                "hello": [{ "s": 2, "p": ["HH", "AH0", "L", "OW1"] }],
                "world": [{ "s": 1, "p": ["W", "ER1", "L", "D"] }],
                "xyz": [{ "s": 1, "p": ["K", "K", "K"] }],
                "alpha": [{ "s": 2, "p": ["AA0", "L", "F", "AH0"] }]
              }
            }
            """;
        File.WriteAllText(_testJsonPath, json);
    }

    #region TryGetRhymeKey

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns a key for a known word with stress.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_KnownWordWithStress_ReturnsKey_Test()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
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
    public void TryGetRhymeKey_UnknownWord_ReturnsFalse_Test()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
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
    public void TryGetRhymeKey_WordsWithSameRhyme_ReturnSameKey_Test()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        provider.TryGetRhymeKey("night", out var nightKey);
        provider.TryGetRhymeKey("light", out var lightKey);

        // Assert
        Assert.Equal(nightKey, lightKey);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns different keys for words that do not rhyme.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_WordsWithDifferentRhyme_ReturnDifferentKeys_Test()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
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
    public void TryGetRhymeKey_WordWithOnlySecondaryStress_UsesLastStressedSyllable_Test()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        // Act
        var result = provider.TryGetRhymeKey("alpha", out var key);

        // Assert
        Assert.True(result);
        Assert.Equal("AH", key);
    }

    /// <summary>
    ///     Verifies that TryGetRhymeKey returns null when no phoneme contains a digit.
    /// </summary>
    [Fact]
    public void TryGetRhymeKey_WordWithNoDigitInPhonemes_ReturnsNull_Test()
    {
        // Arrange
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
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
        GC.SuppressFinalize(this);
        if (File.Exists(_testJsonPath))
        {
            File.Delete(_testJsonPath);
        }
    }
}
