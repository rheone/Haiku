namespace Haiku.Services.Tests.Poems.Matchers;

public class PoemMatcherChainTests
{
    [Fact]
    public void Match_NoMatcherMatches_ReturnsFreeform()
    {
        var matcher1 = Substitute.For<IPoemMatcher>();
        matcher1.Priority.Returns(1);
        matcher1.TryMatch(Arg.Any<string[]>(), Arg.Any<int[]>()).Returns((PoemType?)null);

        var chain = new PoemMatcherChain([matcher1]);
        var result = chain.Match(["line"], [5]);

        Assert.Equal(PoemType.Freeform, result);
    }

    [Fact]
    public void Match_MatcherMatches_ReturnsCorrectType()
    {
        var matcher = Substitute.For<IPoemMatcher>();
        matcher.Priority.Returns(1);
        matcher.TryMatch(Arg.Any<string[]>(), Arg.Any<int[]>()).Returns(PoemType.Haiku);

        var chain = new PoemMatcherChain([matcher]);
        var result = chain.Match(["l1", "l2", "l3"], [5, 7, 5]);

        Assert.Equal(PoemType.Haiku, result);
    }

    [Fact]
    public void Match_HigherPriorityWins_ReturnsHigherPriorityResult()
    {
        var lowPriority = Substitute.For<IPoemMatcher>();
        lowPriority.Priority.Returns(10);
        lowPriority.TryMatch(Arg.Any<string[]>(), Arg.Any<int[]>()).Returns(PoemType.NearTraditional);

        var highPriority = Substitute.For<IPoemMatcher>();
        highPriority.Priority.Returns(2);
        highPriority.TryMatch(Arg.Any<string[]>(), Arg.Any<int[]>()).Returns(PoemType.Haiku);

        var chain = new PoemMatcherChain([highPriority, lowPriority]);
        var result = chain.Match(["l1", "l2", "l3"], [5, 7, 5]);

        Assert.Equal(PoemType.Haiku, result);
    }
}
