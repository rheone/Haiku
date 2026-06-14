using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Moderation;

/// <summary>
/// Handles unhiding a poem and logging the moderation action for audit purposes.
/// </summary>
public class UnhidePoemCommandHandler : ICommandHandler<UnhidePoemCommand, bool>
{
    private readonly IPoemRepository _poemRepository;
    private readonly IModerationRepository _moderationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhidePoemCommandHandler"/> class.
    /// </summary>
    /// <param name="poemRepository">The poem repository for poem data access.</param>
    /// <param name="moderationRepository">The moderation repository for audit logging.</param>
    public UnhidePoemCommandHandler(IPoemRepository poemRepository, IModerationRepository moderationRepository)
    {
        _poemRepository = poemRepository;
        _moderationRepository = moderationRepository;
    }

    /// <inheritdoc/>
    /// <returns><c>true</c> if the poem was unhidden; <c>false</c> if the poem was not found.</returns>
    /// <remarks>
    /// <para>
    /// Returns <c>false</c> if the poem does not exist. Otherwise sets
    /// <see cref="Poem.IsHidden"/> to <c>false</c> and persists a
    /// <see cref="ModerationAction"/> with type <see cref="ModerationActionTypes.Unhide"/>
    /// recording the moderator and reason.
    /// </para>
    /// </remarks>
    public async Task<bool> Handle(UnhidePoemCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Return false when the target poem does not exist (no-op).
        var poem = await _poemRepository.GetByIdAsync(request.PoemId, cancellationToken);
        if (poem == null)
        {
            return false;
        }

        poem.IsHidden = false;
        await _poemRepository.SaveAsync(poem, cancellationToken);

        // Persist an audit trail recording who unhid the poem and why.
        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Unhide,
            TargetType = TargetTypes.Poem,
            TargetId = request.PoemId,
            ActionedBy = new User { Id = request.ActionedByUserId },
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }
}
