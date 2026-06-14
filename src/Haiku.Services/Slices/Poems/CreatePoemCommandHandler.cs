using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using Haiku.Services.Poems;
using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Handles poem creation: type detection, syllable counting, persistence, and hashtag-based tag extraction.
/// </summary>
public class CreatePoemCommandHandler : ICommandHandler<CreatePoemCommand, Poem>
{
    private readonly IPoemRepository _poemRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IPoemInputService _poemInputService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePoemCommandHandler"/> class.
    /// </summary>
    /// <param name="poemRepository">Repository for persisting <see cref="Poem"/> entities.</param>
    /// <param name="tagRepository">Repository for get-or-create operations on hashtag-based <see cref="Tag"/> entities extracted from poem content.</param>
    /// <param name="poemInputService">Service that validates, normalizes, and counts syllables in poem content before persistence.</param>
    public CreatePoemCommandHandler(
        IPoemRepository poemRepository,
        ITagRepository tagRepository,
        IPoemInputService poemInputService
    )
    {
        _poemRepository = poemRepository;
        _tagRepository = tagRepository;
        _poemInputService = poemInputService;
    }

    /// <summary>
    /// Creates and persists a new poem, auto-detecting its type if not specified, counting total syllables,
    /// and extracting <c>#tag</c> markers from the content into <see cref="Tag"/> entities.
    /// </summary>
    /// <param name="request">The command containing author, content, and optional metadata.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The newly created <see cref="Poem"/> entity.</returns>
    /// <exception cref="InvalidOperationException">Thrown when poem content fails validation (e.g., empty content, excessive syllable count, or unresolvable lines).</exception>
    public async Task<Poem> Handle(CreatePoemCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Validate and normalize the poem content (syllable counting, line splitting, etc.).
        var result = _poemInputService.Process(request.Content);
        if (!result.IsValid)
        {
            throw new InvalidOperationException($"Poem validation failed: {string.Join("; ", result.Errors)}");
        }

        var poemType = request.PoemType ?? result.DetectedType ?? PoemType.Freeform;

        var poem = new Poem
        {
            Id = Guid.NewGuid(),
            Author = new User { Id = request.AuthorId },
            Content = result.NormalizedContent,
            PoemType = poemType,
            TotalSyllables = result.TotalSyllables,
            IsDraft = request.IsDraft,
            IsHidden = false,
            CreatedAt = DateTime.UtcNow,
        };

        await _poemRepository.SaveAsync(poem, cancellationToken);

        foreach (var tag in ExtractTags(request.Content))
        {
            await _tagRepository.GetOrCreateAsync(tag, cancellationToken);
        }

        return poem;
    }

    // Extracts unique, lowercased hashtag names (e.g., #Nature -> "nature") from poem content.
    // Tags are embedded inline using the #tag convention in the poem text.
    private static List<string> ExtractTags(string content)
    {
        return content
            .Split(' ', '\n')
            .Where(w => w.StartsWith('#'))
            .Select(w => w.TrimStart('#').ToLowerInvariant())
            .Where(w => !string.IsNullOrEmpty(w))
            .Distinct()
            .ToList();
    }
}
