using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using NHibernate.Linq;

namespace Haiku.Services.Haiku;

public class HaikuService
{
    private readonly IHaikuRepository _haikuRepository;
    private readonly ITagRepository _tagRepository;
    private readonly PoemEngine _poemEngine;

    public HaikuService(
        IHaikuRepository haikuRepository,
        ITagRepository tagRepository,
        PoemEngine poemEngine)
    {
        _haikuRepository = haikuRepository;
        _tagRepository = tagRepository;
        _poemEngine = poemEngine;
    }

    public async Task<Poem> CreateAsync(
        Guid authorId, string content, PoemType poemType, int totalSyllables, bool isDraft)
    {
        var poem = new Poem
        {
            Id = Guid.NewGuid(),
            Author = new User { Id = authorId },
            Content = content,
            PoemType = poemType,
            TotalSyllables = totalSyllables,
            IsDraft = isDraft,
            IsHidden = false,
            CreatedAt = DateTime.UtcNow
        };

        await _haikuRepository.SaveAsync(poem);

        foreach (var tag in ExtractTags(content))
        {
            await _tagRepository.GetOrCreateAsync(tag);
        }

        return poem;
    }

    public async Task<Poem> CreateAsync(
        Guid authorId, string content, bool isDraft = false)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var totalSyllables = lines.Sum(l => _poemEngine.CountLineSyllables(l));
        var detectedType = DetectPoemType(content);

        return await CreateAsync(authorId, content, detectedType, totalSyllables, isDraft);
    }

    public async Task<Poem?> GetByIdAsync(Guid id) =>
        await _haikuRepository.GetByIdAsync(id);

    public async Task DeleteAsync(Poem poem)
    {
        poem.DeletedAt = DateTime.UtcNow;
        await _haikuRepository.SaveAsync(poem);
    }

    public static List<string> ExtractTags(string content)
    {
        return content.Split(' ', '\n')
            .Where(w => w.StartsWith('#'))
            .Select(w => w.TrimStart('#').ToLowerInvariant())
            .Where(w => !string.IsNullOrEmpty(w))
            .Distinct()
            .ToList();
    }

    public static PoemType DetectPoemType(string content, List<int> lineSyllableCounts)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var lineCount = lines.Length;

        if (lineCount == 1 && lineSyllableCounts.Count == 1 && lineSyllableCounts[0] <= 17)
            return PoemType.Monoku;

        if (lineCount == 3 && lineSyllableCounts.Count == 3)
        {
            if (lineSyllableCounts.SequenceEqual(new[] { 5, 7, 5 }))
                return PoemType.Haiku;
            if (lineSyllableCounts.SequenceEqual(new[] { 3, 5, 3 }))
                return PoemType.Minimalist;
            if (lineSyllableCounts.SequenceEqual(new[] { 2, 3, 2 }))
                return PoemType.Compressed;
            if (lineSyllableCounts.SequenceEqual(new[] { 4, 6, 4 }))
                return PoemType.NearTraditional;
            if (lineSyllableCounts.Distinct().Count() == 1)
                return PoemType.EqualLine;
        }

        if (lineCount == 5 && lineSyllableCounts.Count == 5)
        {
            if (lineSyllableCounts.SequenceEqual(new[] { 5, 7, 5, 7, 7 }))
                return PoemType.Tanka;
        }

        if (lineCount >= 1 && lineCount <= 5 && lineSyllableCounts.Count == lineCount)
        {
            if (lineSyllableCounts.Distinct().Count() == 1)
                return PoemType.EqualLine;
        }

        return PoemType.Freeform;
    }

    public PoemType DetectPoemType(string content)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return _poemEngine.DetectPoemType(lines) ?? PoemType.Freeform;
    }
}
