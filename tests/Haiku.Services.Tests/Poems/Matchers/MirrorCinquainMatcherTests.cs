namespace Haiku.Services.Tests.Poems.Matchers;

/// <summary>
/// Tests for <see cref="Haiku.Services.Poems.Matchers.MirrorCinquainMatcher"/> — verifying
/// the 2-4-6-8-2-2-8-6-4-2 ten-line palindromic syllable pattern is correctly
/// identified as a mirror cinquain and that shorter patterns are rejected.
/// </summary>
public class MirrorCinquainMatcherTests
{
    private readonly MirrorCinquainMatcher _matcher = new();

    [Fact]
    public void TryMatch_ValidPattern_ReturnsMirrorCinquain()
    {
        var result = _matcher.TryMatch(
            ["l1", "l2", "l3", "l4", "l5", "l6", "l7", "l8", "l9", "l10"],
            [2, 4, 6, 8, 2, 2, 8, 6, 4, 2]
        );
        Assert.Equal(PoemType.MirrorCinquain, result);
    }

    [Fact]
    public void TryMatch_NineLines_ReturnsNull()
    {
        var result = _matcher.TryMatch(["l1", "l2", "l3", "l4", "l5", "l6", "l7", "l8", "l9"], [2, 4, 6, 8, 2, 2, 8, 6, 4]);
        Assert.Null(result);
    }
}
