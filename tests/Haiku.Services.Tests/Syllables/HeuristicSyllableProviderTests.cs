namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>HeuristicSyllableProvider</c> — verifying English syllable counting heuristics.
/// </summary>
public class HeuristicSyllableProviderTests
{
    private readonly HeuristicSyllableProvider _provider = new();

    #region TryCountSyllables

    /// <summary>
    ///     Verifies that a word ending in consonant+le (e.g., "table") counts the final 'e'
    ///     as a separate syllable rather than applying the silent-e rule incorrectly.
    /// </summary>
    [Fact]
    public void TryCountSyllables_ConsonantLeEnding_CountsFinalSyllable_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var result = _provider.TryCountSyllables("table", out var syllableResult);

        // Assert
        Assert.True(result);
        Assert.Equal(2, syllableResult!.Count);
        Assert.Equal("Heuristic", syllableResult.Tier);
    }

    /// <summary>
    ///     Verifies that "people" counts the consonant+le ending as a separate syllable.
    /// </summary>
    [Fact]
    public void TryCountSyllables_People_CountsTwo_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("people", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that "cradle" counts the consonant+le ending as a separate syllable.
    /// </summary>
    [Fact]
    public void TryCountSyllables_Cradle_CountsTwo_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("cradle", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that "bottle" counts the consonant+le ending as a separate syllable.
    /// </summary>
    [Fact]
    public void TryCountSyllables_Bottle_CountsTwo_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("bottle", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that "ale" does NOT trigger the consonant+le rule (vowel before "le"),
    ///     so the silent 'e' correctly subtracts for a 1-syllable result.
    /// </summary>
    [Fact]
    public void TryCountSyllables_Ale_NotConsonantLe_CountsOne_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("ale", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(1, result!.Count);
    }

    /// <summary>
    ///     Verifies that a simple two-syllable word is counted correctly.
    /// </summary>
    [Fact]
    public void TryCountSyllables_SimpleWord_ReturnsCount_Test()
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
    public void TryCountSyllables_ReturnsAtLeastOne_Test()
    {
        // Arrange
        // _provider initialized in field

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
    public void TryCountSyllables_SilentE_SubtractsOne_Test()
    {
        // Arrange
        // _provider initialized in field

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
    public void TryCountSyllables_ConsecutiveVowels_CountAsOne_Test()
    {
        // Arrange
        // _provider initialized in field

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
    public void TryCountSyllables_TreatsYAsVowel_Test()
    {
        // Arrange
        // _provider initialized in field

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
    public void TryCountSyllables_EmptyWord_ReturnsFalse_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    /// <summary>
    ///     Verifies that whitespace-only input returns false.
    /// </summary>
    [Fact]
    public void TryCountSyllables_Whitespace_ReturnsFalse_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("   ", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    /// <summary>
    ///     Verifies that non-alphabetic characters are stripped before counting,
    ///     so "don't" counts syllables for "dont".
    /// </summary>
    [Fact]
    public void TryCountSyllables_StripsNonAlpha_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("don't", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal("don't", result!.Word);
        Assert.Equal(1, result.Count);
    }

    /// <summary>
    ///     Verifies that input with only non-alphabetic characters returns false.
    /// </summary>
    [Fact]
    public void TryCountSyllables_OnlyNonAlpha_ReturnsFalse_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("123!@#", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    /// <summary>
    ///     Verifies that a single vowel letter counts as one syllable.
    /// </summary>
    [Fact]
    public void TryCountSyllables_SingleVowelLetter_CountsOne_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("a", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(1, result!.Count);
    }

    /// <summary>
    ///     Verifies that a word with a trailing 'y' after a consonant
    ///     counts both vowel groups correctly (e.g., "happy" = hap-py).
    /// </summary>
    [Fact]
    public void TryCountSyllables_TrailingYAfterConsonant_CountsTwo_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("happy", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that a word with adjacent vowel letters spanning
    ///     a consonant boundary counts both vowel groups separately.
    /// </summary>
    [Fact]
    public void TryCountSyllables_WordWithTwoVowelGroups_CountsTwo_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("open", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(2, result!.Count);
    }

    /// <summary>
    ///     Verifies that triply-consecutive vowels are still counted as
    ///     a single vowel group.
    /// </summary>
    [Fact]
    public void TryCountSyllables_ThreeConsecutiveVowels_CountsAsOneGroup_Test()
    {
        // Arrange
        // _provider initialized in field

        // Act
        var success = _provider.TryCountSyllables("beau", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(1, result!.Count);
    }

    #endregion
}
