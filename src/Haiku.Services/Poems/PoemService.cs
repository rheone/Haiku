using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using Haiku.Services.Haiku;
using Haiku.Services.Poems.Matchers;

namespace Haiku.Services.Poems;

/// <summary>
/// Manages poem creation, retrieval, soft-deletion, tag extraction, and type detection.
/// </summary>
public class PoemService
{
    private readonly IPoemRepository _poemRepository;
    private readonly ITagRepository _tagRepository;
    private readonly PoemEngine _poemEngine;
    private readonly IPoemMatcherChain _matcherChain;

    /// <summary>
    /// Initializes a new instance of the <see cref="PoemService"/> class.
    /// </summary>
    /// <param name="poemRepository">Repository for poem entities.</param>
    /// <param name="tagRepository">Repository for tag entities.</param>
    /// <param name="poemEngine">Engine for syllable counting and poem type detection.</param>
    /// <param name="matcherChain">Chain of matchers for poem type detection.</param>
    public PoemService(
        IPoemRepository poemRepository,
        ITagRepository tagRepository,
        PoemEngine poemEngine,
        IPoemMatcherChain matcherChain
    )
    {
        _poemRepository = poemRepository;
        _tagRepository = tagRepository;
        _poemEngine = poemEngine;
        _matcherChain = matcherChain;
    }

    /// <summary>
    /// Creates a poem with an explicitly specified type and syllable count.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <param name="content">The full poem text.</param>
    /// <param name="poemType">The declared poem type.</param>
    /// <param name="totalSyllables">The total syllable count for the poem.</param>
    /// <param name="isDraft">Whether the poem is saved as a draft.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The newly created <see cref="Poem"/>.</returns>
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

    /// <summary>
    /// Creates a poem with auto-detection of type and syllable count via <see cref="PoemEngine"/>.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <param name="content">The full poem text.</param>
    /// <param name="isDraft">Whether the poem is saved as a draft. Defaults to <c>false</c>.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The newly created <see cref="Poem"/>.</returns>
    public async Task<Poem> CreateAsync(
        Guid authorId,
        string content,
        bool isDraft = false,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var totalSyllables = lines.Sum(l => _poemEngine.CountLineSyllables(l));
        var detectedType = DetectPoemType(content);

        return await CreateAsync(authorId, content, detectedType, totalSyllables, isDraft, cancellationToken);
    }

    /// <summary>
    /// Retrieves a poem by its identifier.
    /// </summary>
    /// <param name="id">The poem ID.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The <see cref="Poem"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Poem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _poemRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Soft-deletes a poem by setting its deletion timestamp.
    /// </summary>
    /// <param name="poem">The poem to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync(Poem poem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        poem.DeletedAt = DateTime.UtcNow;
        await _poemRepository.SaveAsync(poem, cancellationToken);
    }

    /// <summary>
    /// Extracts unique hashtags from poem content.
    /// </summary>
    /// <param name="content">The poem text.</param>
    /// <returns>A list of tag strings without the leading <c>#</c>, lowercased and deduplicated.</returns>
    public static List<string> ExtractTags(string content)
    {
        return content
            .Split(' ', '\n')
            .Where(w => w.StartsWith('#'))
            .Select(w => w.TrimStart('#').ToLowerInvariant())
            .Where(w => !string.IsNullOrEmpty(w))
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Detects the poem type from content using pre-computed per-line syllable counts.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This static overload applies syllable-pattern matching rules directly without a <see cref="PoemEngine"/> instance.
    /// It detects all 15 defined poem types in priority order and returns <see cref="PoemType.Freeform"/> for anything else.
    /// </para>
    /// </remarks>
    /// <param name="content">The full poem text.</param>
    /// <param name="lineSyllableCounts">Pre-computed syllable counts for each non-empty line.</param>
    /// <returns>The detected <see cref="PoemType"/>.</returns>
    public static PoemType DetectPoemType(string content, List<int> lineSyllableCounts)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var lineCount = lines.Length;
        var counts = lineSyllableCounts;

        if (lineCount == 1 && counts.Count == 1 && counts[0] >= 4 && counts[0] <= 17)
        {
            return PoemType.Monoku;
        }

        if (lineCount == 3 && counts.Count == 3)
        {
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 5)
            {
                return PoemType.Haiku;
            }
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 7)
            {
                return PoemType.Katauta;
            }
            if (counts[0] == 3 && counts[1] == 5 && counts[2] == 3)
            {
                return PoemType.AmericanLune;
            }
            if (counts[0] == 5 && counts[1] == 3 && counts[2] == 5)
            {
                return PoemType.KellyLune;
            }
            if (counts[0] == 2 && counts[1] == 3 && counts[2] == 2)
            {
                return PoemType.Compressed;
            }
            if (counts[0] == 4 && counts[1] == 6 && counts[2] == 4)
            {
                return PoemType.NearTraditional;
            }
        }

        if (lineCount == 5 && counts.Count == 5)
        {
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 5 && counts[3] == 7 && counts[4] == 7)
            {
                return PoemType.Tanka;
            }
            if (counts[0] == 2 && counts[1] == 4 && counts[2] == 6 && counts[3] == 8 && counts[4] == 2)
            {
                return PoemType.AmericanCinquain;
            }
            if (counts[0] == 2 && counts[1] == 8 && counts[2] == 6 && counts[3] == 4 && counts[4] == 2)
            {
                return PoemType.ReverseCinquain;
            }
        }

        if (lineCount == 6 && counts.Count == 6)
        {
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 7 && counts[3] == 5 && counts[4] == 7 && counts[5] == 7)
            {
                return PoemType.Sedoka;
            }
        }

        if (lineCount == 9 && counts.Count == 9)
        {
            if (
                counts[0] == 2
                && counts[1] == 4
                && counts[2] == 6
                && counts[3] == 8
                && counts[4] == 2
                && counts[5] == 8
                && counts[6] == 6
                && counts[7] == 4
                && counts[8] == 2
            )
            {
                return PoemType.ButterflyCinquain;
            }
        }

        if (lineCount == 10 && counts.Count == 10)
        {
            if (
                counts[0] == 2
                && counts[1] == 4
                && counts[2] == 6
                && counts[3] == 8
                && counts[4] == 2
                && counts[5] == 2
                && counts[6] == 8
                && counts[7] == 6
                && counts[8] == 4
                && counts[9] == 2
            )
            {
                return PoemType.MirrorCinquain;
            }
        }

        if (lineCount >= 7 && lineCount % 2 == 1)
        {
            var choka = true;
            for (var i = 0; i < lineCount - 3; i++)
            {
                var expected = i % 2 == 0 ? 5 : 7;
                if (counts[i] != expected)
                {
                    choka = false;
                    break;
                }
            }

            if (choka && counts[^3] == 5 && counts[^2] == 7 && counts[^1] == 7)
            {
                return PoemType.Choka;
            }
        }

        if (lineCount >= 2 && counts.Count == lineCount)
        {
            var first = counts[0];
            if (counts.Skip(1).All(c => c == first))
            {
                return PoemType.Isosyllabic;
            }
        }

        return PoemType.Freeform;
    }

    /// <summary>
    /// Detects the poem type for a single piece of content using the <see cref="IPoemMatcherChain"/> instance.
    /// </summary>
    /// <param name="content">The full poem text.</param>
    /// <returns>The detected <see cref="PoemType"/>, or <see cref="PoemType.Freeform"/> if no specific pattern matches.</returns>
    public PoemType DetectPoemType(string content)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var counts = lines.Select(l => _poemEngine.CountLineSyllables(l)).ToArray();
        return _matcherChain.Match(lines, counts);
    }
}
