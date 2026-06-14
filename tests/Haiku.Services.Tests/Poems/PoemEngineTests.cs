using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;
using Haiku.Services.Syllables.Providers;
using NewSyllableEngine = Haiku.Services.Syllables.SyllableEngine;
using ServicesPoemEngine = Haiku.Services.Haiku.PoemEngine;

namespace Haiku.Services.Tests.Poems;

public class PoemEngineTests
{
    private static ServicesPoemEngine CreateEngine(
        NewSyllableEngine? syllableEngine = null,
        PoemClassifierChain? classifierChain = null,
        IWordTokenizer? tokenizer = null
    )
    {
        var se = syllableEngine ?? new NewSyllableEngine([new HeuristicSyllableProvider()], new WordTokenizer());
        var cc = classifierChain ?? new PoemClassifierChain([new FreeformClassifier()]);
        var tz = tokenizer ?? new WordTokenizer();
        return new ServicesPoemEngine(se, cc, tz);
    }

    #region Analyze

    /// <summary>
    ///     Verifies that Analyze uses the classifier chain to detect the poem type.
    /// </summary>
    [Fact]
    public void Analyze_UsesClassifierChain()
    {
        // Arrange
        var classifier = Substitute.For<IPoemClassifier>();
        classifier.Priority.Returns(100);
        classifier
            .TryClassify(Arg.Any<string[]>(), Arg.Any<int[]>(), Arg.Any<TokenizedLine[]>(), out Arg.Any<PoemDefinition?>())
            .Returns(x =>
            {
                x[3] = new PoemDefinition { Type = PoemType.Haiku, LineCount = 3 };
                return true;
            });

        var chain = new PoemClassifierChain([classifier]);
        var tokenizer = Substitute.For<IWordTokenizer>();
        tokenizer.Tokenize(Arg.Any<string>()).Returns(new TokenizedLine { Words = ["test"], WordCount = 1 });

        var engine = CreateEngine(classifierChain: chain, tokenizer: tokenizer);

        // Act
        var result = engine.Analyze("hello world", "foo bar", "baz");

        // Assert
        Assert.Equal(PoemType.Haiku, result.Type);
    }

    #endregion
}
