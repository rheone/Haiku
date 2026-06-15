namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>CustomDictionaryProvider</c> — verifying syllable lookups from an in-memory dictionary.
/// </summary>
public class CustomDictionaryProviderTests
{
    #region TryCountSyllables

    /// <summary>
    ///     Verifies that a known word returns its custom syllable count and metadata.
    /// </summary>
    [Fact]
    public void TryCountSyllables_KnownWord_ReturnsCustomCount_Test()
    {
        // Arrange
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "haiku", 2 } };
        var provider = new CustomDictionaryProvider(dict);

        // Act
        var success = provider.TryCountSyllables("haiku", out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("haiku", result!.Word);
        Assert.Equal(2, result.Count);
        Assert.Equal("Custom", result.Tier);
    }

    /// <summary>
    ///     Verifies that an unknown word returns false and a null result with an empty dictionary.
    /// </summary>
    [Fact]
    public void TryCountSyllables_UnknownWord_ReturnsFalse_Test()
    {
        // Arrange
        var provider = new CustomDictionaryProvider();

        // Act
        var success = provider.TryCountSyllables("xyzzy", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    /// <summary>
    ///     Verifies that lookups are case-insensitive when the backing dictionary uses <c>OrdinalIgnoreCase</c>.
    /// </summary>
    [Fact]
    public void TryCountSyllables_CaseInsensitive_Test()
    {
        // Arrange
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "Haiku", 2 } };
        var provider = new CustomDictionaryProvider(dict);

        // Act
        var success = provider.TryCountSyllables("HAIKU", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    #endregion
}
