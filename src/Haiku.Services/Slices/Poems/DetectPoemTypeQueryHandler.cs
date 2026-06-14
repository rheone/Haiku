using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;
using MicroMediator;
using NewSyllableEngine = Haiku.Services.Syllables.SyllableEngine;

namespace Haiku.Services.Slices.Poems;

public class DetectPoemTypeQueryHandler : IQueryHandler<DetectPoemTypeQuery, PoemType>
{
    private readonly PoemClassifierChain _classifierChain;
    private readonly NewSyllableEngine _syllableEngine;
    private readonly IWordTokenizer _tokenizer;

    public DetectPoemTypeQueryHandler(
        PoemClassifierChain classifierChain,
        NewSyllableEngine syllableEngine,
        IWordTokenizer tokenizer
    )
    {
        _classifierChain = classifierChain;
        _syllableEngine = syllableEngine;
        _tokenizer = tokenizer;
    }

    public Task<PoemType> Handle(DetectPoemTypeQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var lines = request.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        int[] syllableCounts;

        if (request.LineSyllableCounts != null)
        {
            syllableCounts = request.LineSyllableCounts.ToArray();
        }
        else
        {
            syllableCounts = lines.Select(l => _syllableEngine.CountLineSyllables(l).TotalSyllables).ToArray();
        }

        var tokenizedLines = lines.Select(l => _tokenizer.Tokenize(l)).ToArray();
        var definition = _classifierChain.Match(lines, syllableCounts, tokenizedLines);

        return Task.FromResult(definition.Type);
    }
}
