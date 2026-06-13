using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Tags;

/// <summary>
/// Handles get-or-create operations for tags, delegating to <see cref="ITagRepository"/>.
/// </summary>
public class GetOrCreateTagCommandHandler : ICommandHandler<GetOrCreateTagCommand, Tag>
{
    private readonly ITagRepository _tagRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrCreateTagCommandHandler"/> class.
    /// </summary>
    /// <param name="tagRepository">Repository for tag get-or-create operations.</param>
    public GetOrCreateTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    /// <summary>
    /// Retrieves or creates a tag for the given name.
    /// </summary>
    /// <param name="request">The command containing the tag name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The existing or newly created <see cref="Tag"/> entity.</returns>
    public async Task<Tag> Handle(GetOrCreateTagCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _tagRepository.GetOrCreateAsync(request.TagName, cancellationToken);
    }
}
