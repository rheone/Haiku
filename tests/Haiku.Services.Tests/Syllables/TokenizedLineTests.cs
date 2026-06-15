namespace Haiku.Services.Tests.Syllables;

/// <summary>
///     Tests for <c>TokenizedLine</c> — verifying default values, property initialization, and equality.
/// </summary>
public class TokenizedLineTests
{
    #region DefaultValues

    /// <summary>
    ///     Verifies that a default-constructed <c>TokenizedLine</c> has empty arrays and zero counts.
    /// </summary>
    [Fact]
    public void DefaultValues_AreEmptyArraysAndZero_Test()
    {
        // Arrange
        var line = new TokenizedLine();

        // Act (implicit — construction is the act)

        // Assert
        Assert.Empty(line.Words);
        Assert.Empty(line.WordSyllableCounts);
        Assert.Equal(0, line.TotalSyllables);
        Assert.Equal(0, line.WordCount);
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Verifies that all properties can be set via the object initializer and read back correctly.
    /// </summary>
    [Fact]
    public void Properties_CanBeSetViaInit_Test()
    {
        // Arrange
        var words = new[] { "hello", "world" };
        var counts = new[] { 2, 1 };

        var line = new TokenizedLine
        {
            Words = words,
            WordSyllableCounts = counts,
            TotalSyllables = 3,
            WordCount = 2,
        };

        // Act (implicit — construction is the act)

        // Assert
        Assert.Equal(words, line.Words);
        Assert.Equal(counts, line.WordSyllableCounts);
        Assert.Equal(3, line.TotalSyllables);
        Assert.Equal(2, line.WordCount);
    }

    #endregion

    #region Equality

    /// <summary>
    ///     Verifies that two <c>TokenizedLine</c> instances with the same values are considered equal.
    /// </summary>
    [Fact]
    public void SameValues_AreEqual_Test()
    {
        // Arrange
        var words = new[] { "hello" };
        var counts = new[] { 2 };

        var line1 = new TokenizedLine
        {
            Words = words,
            WordSyllableCounts = counts,
            TotalSyllables = 2,
            WordCount = 1,
        };

        var line2 = new TokenizedLine
        {
            Words = words,
            WordSyllableCounts = counts,
            TotalSyllables = 2,
            WordCount = 1,
        };

        // Act (implicit — construction is the act)

        // Assert
        Assert.Equal(line1, line2);
        Assert.Equal(line1.GetHashCode(), line2.GetHashCode());
    }

    #endregion
}
