using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using Haiku.Services.Poems;
using Haiku.Services.Poems.Matchers;
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
    private readonly IPoemMatcherChain _matcherChain;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePoemCommandHandler"/> class.
    /// </summary>
    /// <param name="poemRepository">Repository for persisting <see cref="Poem"/> entities.</param>
    /// <param name="tagRepository">Repository for get-or-create <see cref="Tag"/> entities.</param>
    /// <param name="poemInputService">Service for processing and validating poem input.</param>
    /// <param name="matcherChain">Chain of matchers for poem type detection.</param>
    public CreatePoemCommandHandler(
        IPoemRepository poemRepository,
        ITagRepository tagRepository,
        IPoemInputService poemInputService,
        IPoemMatcherChain matcherChain
    )
    {
        _poemRepository = poemRepository;
        _tagRepository = tagRepository;
        _poemInputService = poemInputService;
        _matcherChain = matcherChain;
    }

    /// <summary>
    /// Creates and persists a new poem, auto-detecting its type if not specified, counting total syllables,
    /// and extracting <c>#tag</c> markers from the content into <see cref="Tag"/> entities.
    /// </summary>
    /// <param name="request">The command containing author, content, and optional metadata.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The newly created <see cref="Poem"/> entity.</returns>
    public async Task<Poem> Handle(CreatePoemCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

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
