using Haiku.Services.Syllables.Providers;

namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>CmuDictionaryProvider</c> — verifying syllable lookups from a CMU Pronouncing Dictionary file.
/// </summary>
public class CmuDictionaryProviderTests : IDisposable
{
    private readonly string _testDictPath;

    public CmuDictionaryProviderTests()
    {
        _testDictPath = Path.GetTempFileName();
        File.WriteAllLines(
            _testDictPath,
            ["hello  HH AH0 L OW1", "world  W ER1 L D", "silence  S AY1 L AH0 N S", ";;; comment line"]
        );
    }

    #region TryCountSyllables

    /// <summary>
    ///     Verifies that a known word in the dictionary returns its syllable count and metadata.
    /// </summary>
    [Fact]
    public void TryCountSyllables_KnownWord_ReturnsCount()
    {
        // Arrange
        var provider = new CmuDictionaryProvider(_testDictPath);

        // Act
        var success = provider.TryCountSyllables("hello", out var result);

        // Assert
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
        // Arrange
        var provider = new CmuDictionaryProvider(_testDictPath);

        // Act
        var success = provider.TryCountSyllables("xyzzy", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    /// <summary>
    ///     Verifies that comment lines prefixed with <c>;;;</c> are ignored during lookup.
    /// </summary>
    [Fact]
    public void TryCountSyllables_SkipsCommentLines()
    {
        // Arrange
        var provider = new CmuDictionaryProvider(_testDictPath);

        // Act
        var success = provider.TryCountSyllables(";;;", out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Constructor

    /// <summary>
    ///     Verifies that a missing dictionary file path throws <c>FileNotFoundException</c>.
    /// </summary>
    [Fact]
    public void Constructor_MissingFile_ThrowsFileNotFound()
    {
        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => new CmuDictionaryProvider("/nonexistent/path.dic"));
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
