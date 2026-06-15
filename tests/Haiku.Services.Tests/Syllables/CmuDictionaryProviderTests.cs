using System.Text.Json;

namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>CmuDictionaryProvider</c> — verifying syllable lookups from a CMU pronunciation JSON file.
/// </summary>
public sealed class CmuDictionaryProviderTests : IDisposable
{
    private readonly string _testJsonPath;

    public CmuDictionaryProviderTests()
    {
        _testJsonPath = Path.GetTempFileName();
        var json = """
            {
              "_metadata": {
                "source": "https://github.com/cmusphinx/cmudict",
                "commit": "test-fixture",
                "generatedAt": "2026-06-14T00:00:00Z",
                "license": "Public Domain"
              },
              "entries": {
                "hello": [{ "s": 2, "p": ["HH", "AH0", "L", "OW1"] }],
                "world": [{ "s": 1, "p": ["W", "ER1", "L", "D"] }],
                "silence": [{ "s": 2, "p": ["S", "AY1", "L", "AH0", "N", "S"] }],
                "record": [
                  { "s": 2, "p": ["R", "EH1", "K", "ER0", "D"] },
                  { "s": 3, "p": ["R", "IH0", "K", "AO1", "R", "D"] }
                ]
              }
            }
            """;
        File.WriteAllText(_testJsonPath, json);
    }

    #region TryCountSyllables

    /// <summary>
    ///     Verifies that a known word in the dictionary returns its syllable count and tier.
    /// </summary>
    [Fact]
    public void TryCountSyllables_KnownWord_ReturnsCount()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryCountSyllables("hello", out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("hello", result!.Word);
        Assert.Equal(2, result.Count);
        Assert.Equal("CMU", result.Tier);
    }

    /// <summary>
    ///     Verifies that an unknown word returns false and a null result.
    /// </summary>
    [Fact]
    public void TryCountSyllables_UnknownWord_ReturnsFalse()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryCountSyllables("xyzzy", out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    /// <summary>
    ///     Verifies that case-insensitive lookups work (CMU dict is case-insensitive).
    /// </summary>
    [Fact]
    public void TryCountSyllables_CaseInsensitive_Matches()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryCountSyllables("HELLO", out var result);

        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that a word with multiple pronunciations returns the first entry's syllable count.
    /// </summary>
    [Fact]
    public void TryCountSyllables_Homograph_ReturnsFirstEntry()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryCountSyllables("record", out var result);

        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that the <c>_metadata</c> section is ignored and does not interfere with lookups.
    /// </summary>
    [Fact]
    public void TryCountSyllables_MetadataIgnored_DoesNotCreateEntry()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryCountSyllables("_metadata", out _);

        Assert.False(success);
    }

    #endregion

    #region Constructor

    /// <summary>
    ///     Verifies that a missing JSON file path throws <c>FileNotFoundException</c>.
    /// </summary>
    [Fact]
    public void Constructor_MissingFile_ThrowsFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => new CmuDictionaryProvider("/nonexistent/path.json"));
    }

    /// <summary>
    ///     Verifies that invalid JSON throws <c>JsonException</c>.
    /// </summary>
    [Fact]
    public void Constructor_InvalidJson_ThrowsJsonException()
    {
        File.WriteAllText(_testJsonPath, "not valid json");
        Assert.Throws<JsonException>(() => new CmuDictionaryProvider(_testJsonPath));
    }

    /// <summary>
    ///     Verifies that an empty file throws <c>JsonException</c>.
    /// </summary>
    [Fact]
    public void Constructor_EmptyFile_ThrowsJsonException()
    {
        File.WriteAllText(_testJsonPath, "");
        Assert.Throws<JsonException>(() => new CmuDictionaryProvider(_testJsonPath));
    }

    /// <summary>
    ///     Verifies that a JSON file without an <c>entries</c> property throws <c>InvalidOperationException</c>.
    /// </summary>
    [Fact]
    public void Constructor_MissingEntriesProperty_ThrowsInvalidOperation()
    {
        File.WriteAllText(_testJsonPath, """{ "metadata": { "version": 1 } }""");
        var ex = Assert.Throws<InvalidOperationException>(() => new CmuDictionaryProvider(_testJsonPath));
        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Verifies that a JSON file with an empty <c>entries</c> object throws <c>InvalidOperationException</c>.
    /// </summary>
    [Fact]
    public void Constructor_EmptyEntriesObject_ThrowsInvalidOperation()
    {
        File.WriteAllText(_testJsonPath, """{ "entries": {} }""");
        var ex = Assert.Throws<InvalidOperationException>(() => new CmuDictionaryProvider(_testJsonPath));
        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region TryGetWordsBySyllableCount

    /// <summary>
    ///     Verifies that words with a specific syllable count can be retrieved.
    /// </summary>
    [Fact]
    public void TryGetWordsBySyllableCount_KnownCount_ReturnsWords()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryGetWordsBySyllableCount(1, out var words);

        Assert.True(success);
        Assert.Contains("world", words);
        Assert.DoesNotContain("hello", words);
    }

    /// <summary>
    ///     Verifies that requesting a syllable count with no matches returns false.
    /// </summary>
    [Fact]
    public void TryGetWordsBySyllableCount_NoMatch_ReturnsFalse()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryGetWordsBySyllableCount(99, out var words);

        Assert.False(success);
        Assert.Empty(words);
    }

    #endregion

    #region TryGetWords

    /// <summary>
    ///     Verifies that random words can be retrieved.
    /// </summary>
    [Fact]
    public void TryGetWords_ReturnsRequestedCount()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);
        var deterministic = new Random(42);

        var success = provider.TryGetWords(2, out var words, deterministic);

        Assert.True(success);
        Assert.Equal(2, words.Count);
    }

    /// <summary>
    ///     Verifies that deterministic seeding produces the same result.
    /// </summary>
    [Fact]
    public void TryGetWords_DeterministicSeed_ReturnsSameOrder()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success1 = provider.TryGetWords(3, out var words1, new Random(42));
        var success2 = provider.TryGetWords(3, out var words2, new Random(42));

        Assert.True(success1);
        Assert.True(success2);
        Assert.Equal(words1, words2);
    }

    #endregion

    #region TryGetSpecificWords

    /// <summary>
    ///     Verifies that one word per requested syllable count is returned, in order.
    /// </summary>
    [Fact]
    public void TryGetSpecificWords_ReturnsWordPerCount()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);
        var deterministic = new Random(42);

        var success = provider.TryGetSpecificWords([1, 2], out var words, deterministic);

        Assert.True(success);
        Assert.Equal(2, words.Count);
    }

    /// <summary>
    ///     Verifies that requesting counts where no match exists returns false.
    /// </summary>
    [Fact]
    public void TryGetSpecificWords_NoMatchForCount_ReturnsFalse()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryGetSpecificWords([1, 99], out var words);

        Assert.False(success);
        Assert.Empty(words);
    }

    #endregion

    #region TryGetPhonemes

    /// <summary>
    ///     Verifies that a known word returns its phoneme array.
    /// </summary>
    [Fact]
    public void TryGetPhonemes_KnownWord_ReturnsPhonemes()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryGetPhonemes("hello", out var phonemes);

        Assert.True(success);
        Assert.Equal(["HH", "AH0", "L", "OW1"], phonemes!);
    }

    /// <summary>
    ///     Verifies that an unknown word returns false.
    /// </summary>
    [Fact]
    public void TryGetPhonemes_UnknownWord_ReturnsFalse()
    {
        var provider = new CmuDictionaryProvider(_testJsonPath);

        var success = provider.TryGetPhonemes("xyzzy", out _);

        Assert.False(success);
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
