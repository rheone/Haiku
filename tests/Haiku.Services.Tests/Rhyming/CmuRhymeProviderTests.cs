using Haiku.Services.Rhyming.Providers;
using Haiku.Services.Syllables.Providers;

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

    [Fact]
    public void TryGetRhymeKey_KnownWordWithStress_ReturnsKey()
    {
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        var result = provider.TryGetRhymeKey("night", out var key);

        Assert.True(result);
        Assert.Equal("AY T", key);
    }

    [Fact]
    public void TryGetRhymeKey_UnknownWord_ReturnsFalse()
    {
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        var result = provider.TryGetRhymeKey("unknownword", out var key);

        Assert.False(result);
        Assert.Null(key);
    }

    [Fact]
    public void TryGetRhymeKey_WordsWithSameRhyme_ReturnSameKey()
    {
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        provider.TryGetRhymeKey("night", out var nightKey);
        provider.TryGetRhymeKey("light", out var lightKey);

        Assert.Equal(nightKey, lightKey);
    }

    [Fact]
    public void TryGetRhymeKey_WordsWithDifferentRhyme_ReturnDifferentKeys()
    {
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        provider.TryGetRhymeKey("moon", out var moonKey);
        provider.TryGetRhymeKey("day", out var dayKey);

        Assert.NotEqual(moonKey, dayKey);
    }

    [Fact]
    public void TryGetRhymeKey_WordWithOnlySecondaryStress_UsesLastStressedSyllable()
    {
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        var result = provider.TryGetRhymeKey("alpha", out var key);

        Assert.True(result);
        Assert.Equal("AH", key);
    }

    [Fact]
    public void TryGetRhymeKey_WordWithNoDigitInPhonemes_ReturnsNull()
    {
        var dictProvider = new CmuDictionaryProvider(_testJsonPath);
        var provider = new CmuRhymeProvider(dictProvider);

        var result = provider.TryGetRhymeKey("xyz", out var key);

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
