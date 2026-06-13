using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Moderation;

/// <summary>
/// Handles hiding a poem and logging the moderation action for audit purposes.
/// </summary>
public class HidePoemCommandHandler : ICommandHandler<HidePoemCommand, bool>
{
    private readonly IHaikuRepository _haikuRepository;
    private readonly IModerationRepository _moderationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="HidePoemCommandHandler"/> class.
    /// </summary>
    /// <param name="haikuRepository">The haiku repository for poem data access.</param>
    /// <param name="moderationRepository">The moderation repository for audit logging.</param>
    public HidePoemCommandHandler(IHaikuRepository haikuRepository, IModerationRepository moderationRepository)
    {
        _haikuRepository = haikuRepository;
        _moderationRepository = moderationRepository;
    }

    /// <inheritdoc/>
    /// <returns><c>true</c> if the poem was hidden; <c>false</c> if the poem was not found.</returns>
    /// <remarks>
    /// <para>
    /// Returns <c>false</c> if the poem does not exist. Otherwise sets
    /// <see cref="Poem.IsHidden"/> to <c>true</c> and persists a
    /// <see cref="ModerationAction"/> with type <see cref="ModerationActionTypes.Hide"/>
    /// recording the moderator and reason.
    /// </para>
    /// </remarks>
    public async Task<bool> Handle(HidePoemCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var poem = await _haikuRepository.GetByIdAsync(request.PoemId, cancellationToken);
        if (poem == null)
        {
            return false;
        }

        poem.IsHidden = true;
        await _haikuRepository.SaveAsync(poem, cancellationToken);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Hide,
            TargetType = TargetTypes.Haiku,
            TargetId = request.PoemId,
            ActionedBy = new User { Id = request.ActionedByUserId },
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }
}
