using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Tests.Poems.Classifiers;

public class PoemClassifierChainTests
{
    #region Match

    /// <summary>
    /// Verifies that when no classifier matches, Match returns Freeform as the fallback type.
    /// </summary>
    [Fact]
    public void Match_NoClassifierMatches_ReturnsFreeform()
    {
        // Arrange
        var classifier = Substitute.For<IPoemClassifier>();
        classifier.Priority.Returns(100);
        classifier
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = null;
                return false;
            });

        var chain = new PoemClassifierChain([classifier]);

        // Act
        var result = chain.Match(["line"], [5], []);

        // Assert
        Assert.Equal(PoemType.Freeform, result.Type);
    }

    /// <summary>
    /// Verifies that when a classifier matches, Match returns the correct poem type.
    /// </summary>
    [Fact]
    public void Match_ClassifierMatches_ReturnsCorrectType()
    {
        // Arrange
        var classifier = Substitute.For<IPoemClassifier>();
        classifier.Priority.Returns(100);
        classifier
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = new PoemDefinition { Type = PoemType.Haiku };
                return true;
            });

        var chain = new PoemClassifierChain([classifier]);

        // Act
        var result = chain.Match(["l1", "l2", "l3"], [5, 7, 5], []);

        // Assert
        Assert.Equal(PoemType.Haiku, result.Type);
    }

    /// <summary>
    /// Verifies that Match returns the result from the higher-priority classifier when two classifiers both match.
    /// </summary>
    [Fact]
    public void Match_HigherPriorityWins()
    {
        // Arrange
        var lowPriority = Substitute.For<IPoemClassifier>();
        lowPriority.Priority.Returns(500);
        lowPriority
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = new PoemDefinition { Type = PoemType.Katauta };
                return true;
            });

        var highPriority = Substitute.For<IPoemClassifier>();
        highPriority.Priority.Returns(100);
        highPriority
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = new PoemDefinition { Type = PoemType.Monoku };
                return true;
            });

        var chain = new PoemClassifierChain([highPriority, lowPriority]);

        // Act
        var result = chain.Match(["line"], [5], []);

        // Assert
        Assert.Equal(PoemType.Monoku, result.Type);
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Verifies that constructing a chain with classifiers having duplicate priorities throws.
    /// </summary>
    [Fact]
    public void Constructor_DuplicatePriority_Throws()
    {
        // Arrange
        var c1 = Substitute.For<IPoemClassifier>();
        c1.Priority.Returns(100);
        var c2 = Substitute.For<IPoemClassifier>();
        c2.Priority.Returns(100);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new PoemClassifierChain([c1, c2]));
    }

    #endregion
}
