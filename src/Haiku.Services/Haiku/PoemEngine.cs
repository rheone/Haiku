using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;
using Haiku.Services.Syllables.Providers;
using NewSyllableEngine = Haiku.Services.Syllables.SyllableEngine;

namespace Haiku.Services.Haiku;

public sealed class PoemEngine
{
    private readonly NewSyllableEngine _syllableEngine;
    private readonly PoemClassifierChain _classifierChain;
    private readonly IWordTokenizer _tokenizer;
    private readonly Dictionary<int, List<string>> _wordsBySyllableCount = [];

    public PoemEngine(
        NewSyllableEngine syllableEngine,
        PoemClassifierChain classifierChain,
        IWordTokenizer tokenizer,
        CmuDictionaryProvider? cmuProvider = null
    )
    {
        _syllableEngine = syllableEngine;
        _classifierChain = classifierChain;
        _tokenizer = tokenizer;

        if (cmuProvider != null)
        {
            foreach (var (word, count) in cmuProvider.GetWordsBySyllableCount())
            {
                if (!_wordsBySyllableCount.TryGetValue(count, out var list))
                {
                    list = [];
                    _wordsBySyllableCount[count] = list;
                }

                if (!list.Contains(word))
                {
                    list.Add(word);
                }
            }
        }
    }

    public PoemDefinition Analyze(params string[] lines)
    {
        var normalizedContent = string.Join("\n", lines);
        var tokenizedLines = lines.Select(l => _tokenizer.Tokenize(l)).ToArray();
        var syllableCounts = tokenizedLines
            .Select(t => t.Words.Sum(w => _syllableEngine.CountWordSyllables(w).Count))
            .ToArray();

        var definition = _classifierChain.Match(lines, syllableCounts, tokenizedLines);

        return definition with
        {
            OriginalContent = normalizedContent,
            NormalizedContent = string.Join(" ", lines),
        };
    }

    public string[] GeneratePoem(PoemType type, int? seed = null)
    {
        if (_wordsBySyllableCount.Count == 0)
        {
            throw new InvalidOperationException(
                "CMU dictionary must be loaded before generating poems. Provide CmuDictionaryProvider to constructor."
            );
        }

        var rng = seed.HasValue ? new Random(seed.Value) : new Random();

        return type switch
        {
            PoemType.Freeform => [],
            PoemType.Monoku => [BuildLine(rng.Next(4, 18), rng)],
            PoemType.Isosyllabic => GenerateEqualLinePoem(rng),
            PoemType.Choka => GenerateChokaPoem(rng),
            _ => BuildPoemFromPattern(type, rng),
        };
    }

    private string[] BuildPoemFromPattern(PoemType type, Random rng)
    {
        var pattern = type switch
        {
            PoemType.Haiku => new[] { 5, 7, 5 },
            PoemType.Tanka => new[] { 5, 7, 5, 7, 7 },
            PoemType.Katauta => new[] { 5, 7, 7 },
            PoemType.Sedoka => new[] { 5, 7, 7, 5, 7, 7 },
            PoemType.AmericanLune => new[] { 3, 5, 3 },
            PoemType.KellyLune => new[] { 5, 3, 5 },
            PoemType.AmericanCinquain => new[] { 2, 4, 6, 8, 2 },
            PoemType.ReverseCinquain => new[] { 2, 8, 6, 4, 2 },
            PoemType.MirrorCinquain => new[] { 2, 4, 6, 8, 2, 2, 8, 6, 4, 2 },
            PoemType.ButterflyCinquain => new[] { 2, 4, 6, 8, 2, 8, 6, 4, 2 },
            PoemType.Compressed => new[] { 2, 3, 2 },
            PoemType.NearTraditional => new[] { 4, 6, 4 },
            _ => [],
        };

        return [.. pattern.Select(target => BuildLine(target, rng))];
    }

    private string BuildLine(int targetSyllables, Random rng)
    {
        const int maxAttempts = 2000;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var words = new List<string>();
            var budget = targetSyllables;

            while (budget > 0)
            {
                var candidates = GetCandidateWords(budget, rng);
                if (candidates.Count == 0)
                {
                    break;
                }

                var pick = candidates[rng.Next(candidates.Count)];
                words.Add(pick);
                budget -= _syllableEngine.CountWordSyllables(pick).Count;
            }

            if (budget == 0)
            {
                return string.Join(" ", words);
            }
        }

        return $"[{targetSyllables} syllables]";
    }

    private string[] GenerateEqualLinePoem(Random rng)
    {
        var lineCount = rng.Next(2, 6);
        var syllables = rng.Next(3, 8);
        var lines = new string[lineCount];

        for (var i = 0; i < lineCount; i++)
        {
            lines[i] = BuildLine(syllables, rng);
        }

        return lines;
    }

    private string[] GenerateChokaPoem(Random rng)
    {
        var lineCount = rng.Next(3, 15);
        if (lineCount % 2 == 0)
        {
            lineCount++;
        }

        var lines = new string[lineCount];
        for (var i = 0; i < lineCount - 2; i++)
        {
            lines[i] = BuildLine(i % 2 == 0 ? 5 : 7, rng);
        }

        lines[^2] = BuildLine(5, rng);
        lines[^1] = BuildLine(7, rng);
        return lines;
    }

    private List<string> GetCandidateWords(int maxSyllables, Random rng)
    {
        var pool = Enumerable
            .Range(1, maxSyllables)
            .Where(_wordsBySyllableCount.ContainsKey)
            .SelectMany(n => _wordsBySyllableCount[n])
            .ToList();

        return [.. pool.OrderBy(_ => rng.Next()).Take(50)];
    }
}
