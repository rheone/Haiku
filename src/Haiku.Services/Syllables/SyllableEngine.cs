using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables;

public sealed class SyllableEngine
{
    private readonly IReadOnlyList<ISyllableProvider> _providers;
    private readonly IWordTokenizer _tokenizer;

    public SyllableEngine(IEnumerable<ISyllableProvider> providers, IWordTokenizer tokenizer)
    {
        _providers = providers.ToList();
        _tokenizer = tokenizer;
    }

    public SyllableResult CountWordSyllables(string word)
    {
        foreach (var provider in _providers)
        {
            if (provider.TryCountSyllables(word, out var result))
            {
                return result;
            }
        }

        return new SyllableResult(word, 1, "none");
    }

    public LineSyllableResult CountLineSyllables(string line, int lineNumber = 1)
    {
        var tokenized = _tokenizer.Tokenize(line);
        var wordResults = tokenized.Words.Select(w => CountWordSyllables(w)).ToArray();

        return new LineSyllableResult(lineNumber, line, wordResults.Sum(r => r.Count), wordResults);
    }
}
