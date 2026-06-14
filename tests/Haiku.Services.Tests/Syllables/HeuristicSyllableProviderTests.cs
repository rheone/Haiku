using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables.Providers;

namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>HeuristicSyllableProvider</c> — verifying English syllable counting heuristics.
/// </summary>
public class HeuristicSyllableProviderTests
{
    private readonly HeuristicSyllableProvider _provider = new();

    #region TryCountSyllables

    /// <summary>
    ///     Verifies that a simple two-syllable word is counted correctly.
    /// </summary>
    [Fact]
    public void TryCountSyllables_SimpleWord_ReturnsCount()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var result = _provider.TryCountSyllables("hello", out var syllableResult);

        // Assert
        Assert.True(result);
        Assert.NotNull(syllableResult);
        Assert.Equal("hello", syllableResult.Word);
        Assert.Equal(2, syllableResult.Count);
        Assert.Equal("Heuristic", syllableResult.Tier);
    }

    /// <summary>
    ///     Verifies that every word is assigned at least one syllable.
    /// </summary>
    [Fact]
    public void TryCountSyllables_ReturnsAtLeastOne()
    {
        // Act
        var success = _provider.TryCountSyllables("xyzzy", out var result);

        // Assert
        Assert.True(success);
        Assert.InRange(result!.Count, 1, int.MaxValue);
    }

    /// <summary>
    ///     Verifies that a silent-e ending subtracts one from the vowel count.
    /// </summary>
    [Fact]
    public void TryCountSyllables_SilentE_SubtractsOne()
    {
        // Act
        var success = _provider.TryCountSyllables("make", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(1, result!.Count);
    }

    /// <summary>
    ///     Verifies that consecutive vowels are counted as a single syllable.
    /// </summary>
    [Fact]
    public void TryCountSyllables_ConsecutiveVowels_CountAsOne()
    {
        // Act
        var success = _provider.TryCountSyllables("beauty", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that <c>y</c> is treated as a vowel when no other vowel is present.
    /// </summary>
    [Fact]
    public void TryCountSyllables_TreatsYAsVowel()
    {
        // Act
        var success = _provider.TryCountSyllables("sky", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(1, result!.Count);
    }

    /// <summary>
    ///     Verifies that an empty string returns false and a null result.
    /// </summary>
    [Fact]
    public void TryCountSyllables_EmptyWord_ReturnsFalse()
    {
        // Act
        var success = _provider.TryCountSyllables("", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion
}
