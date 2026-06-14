using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Syllables;
using NewSyllableEngine = Haiku.Services.Syllables.SyllableEngine;

namespace Haiku.Services.Poems;

public class PoemService
{
    private readonly IPoemRepository _poemRepository;
    private readonly ITagRepository _tagRepository;
    private readonly NewSyllableEngine _syllableEngine;
    private readonly PoemClassifierChain _classifierChain;
    private readonly IWordTokenizer _tokenizer;

    public PoemService(
        IPoemRepository poemRepository,
        ITagRepository tagRepository,
        NewSyllableEngine syllableEngine,
        PoemClassifierChain classifierChain,
        IWordTokenizer tokenizer
    )
    {
        _poemRepository = poemRepository;
        _tagRepository = tagRepository;
        _syllableEngine = syllableEngine;
        _classifierChain = classifierChain;
        _tokenizer = tokenizer;
    }

    public async Task<Poem> CreateAsync(
        Guid authorId,
        string content,
        PoemType poemType,
        int totalSyllables,
        bool isDraft,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var poem = new Poem
        {
            Id = Guid.NewGuid(),
            Author = new User { Id = authorId },
            Content = content,
            PoemType = poemType,
            TotalSyllables = totalSyllables,
            IsDraft = isDraft,
            IsHidden = false,
            CreatedAt = DateTime.UtcNow,
        };

        await _poemRepository.SaveAsync(poem, cancellationToken);

        foreach (var tag in ExtractTags(content))
        {
            await _tagRepository.GetOrCreateAsync(tag, cancellationToken);
        }

        return poem;
    }

    public async Task<Poem> CreateAsync(
        Guid authorId,
        string content,
        bool isDraft = false,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var tokenizedLines = lines.Select(l => _tokenizer.Tokenize(l)).ToArray();
        var syllableCounts = tokenizedLines
            .Select(t => t.Words.Sum(w => _syllableEngine.CountWordSyllables(w).Count))
            .ToArray();
        var totalSyllables = syllableCounts.Sum();
        var definition = _classifierChain.Match(lines, syllableCounts, tokenizedLines);

        return await CreateAsync(authorId, content, definition.Type, totalSyllables, isDraft, cancellationToken);
    }

    public async Task<Poem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _poemRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(Poem poem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        poem.DeletedAt = DateTime.UtcNow;
        await _poemRepository.SaveAsync(poem, cancellationToken);
    }

    public static List<string> ExtractTags(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return content
            .Split(' ', '\n')
            .Where(w => w.StartsWith('#'))
            .Select(w => w.TrimStart('#').ToLowerInvariant())
            .Where(w => !string.IsNullOrEmpty(w))
            .Distinct()
            .ToList();
    }
}
