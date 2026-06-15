namespace Haiku.Services.Tests.Rhyming;

public class RhymingEngineTests
{
    #region WordsRhyme

    /// <summary>
    ///     Verifies that WordsRhyme returns true when comparing a word to itself.
    /// </summary>
    [Fact]
    public void WordsRhyme_SameWord_ReturnsTrue()
    {
        // Arrange
        var provider = Substitute.For<IRhymeProvider>();
        var engine = new RhymingEngine([provider]);

        // Act
        var result = engine.WordsRhyme("hello", "hello");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Verifies that WordsRhyme returns true when the provider matches two words.
    /// </summary>
    [Fact]
    public void WordsRhyme_ProviderMatches_ReturnsTrue()
    {
        // Arrange
        var provider = Substitute.For<IRhymeProvider>();
        provider
            .TryGetRhymeKey("night", out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = "AYT";
                return true;
            });
        provider
            .TryGetRhymeKey("light", out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = "AYT";
                return true;
            });

        var engine = new RhymingEngine([provider]);

        // Act
        var result = engine.WordsRhyme("night", "light");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Verifies that WordsRhyme returns false when the provider returns different keys.
    /// </summary>
    [Fact]
    public void WordsRhyme_DifferentKeys_ReturnsFalse()
    {
        // Arrange
        var provider = Substitute.For<IRhymeProvider>();
        provider
            .TryGetRhymeKey("night", out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = "AYT";
                return true;
            });
        provider
            .TryGetRhymeKey("day", out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = "EY";
                return true;
            });

        var engine = new RhymingEngine([provider]);

        // Act
        var result = engine.WordsRhyme("night", "day");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Verifies that WordsRhyme falls back to suffix matching when the provider returns null.
    /// </summary>
    [Fact]
    public void WordsRhyme_FallbackToSuffix_WhenProviderReturnsNull()
    {
        // Arrange
        var provider = Substitute.For<IRhymeProvider>();
        provider.TryGetRhymeKey(Arg.Any<string>(), out Arg.Any<string?>()).Returns(false);

        var engine = new RhymingEngine([provider]);

        // Act
        var result = engine.WordsRhyme("playing", "saying");

        // Assert
        Assert.True(result);
    }

    #endregion

    #region LinesRhyme

    /// <summary>
    ///     Verifies that LinesRhyme returns true when the last words of each line rhyme.
    /// </summary>
    [Fact]
    public void LinesRhyme_LastWordsRhyme_ReturnsTrue()
    {
        // Arrange
        var provider = Substitute.For<IRhymeProvider>();
        provider
            .TryGetRhymeKey("moon", out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = "UW-N";
                return true;
            });
        provider
            .TryGetRhymeKey("june", out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = "UW-N";
                return true;
            });

        var engine = new RhymingEngine([provider]);

        // Act
        var result = engine.LinesRhyme("the shining moon", "warm summer june");

        // Assert
        Assert.True(result);
    }

    #endregion
}
