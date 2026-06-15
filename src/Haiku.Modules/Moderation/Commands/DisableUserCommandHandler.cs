using MicroMediator;

namespace Haiku.Modules.Moderation.Commands;

/// <summary>
/// Handles disabling a user account and logging the moderation action for audit purposes.
/// </summary>
public class DisableUserCommandHandler : ICommandHandler<DisableUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IModerationRepository _moderationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository for data access.</param>
    /// <param name="moderationRepository">The moderation repository for audit logging.</param>
    public DisableUserCommandHandler(IUserRepository userRepository, IModerationRepository moderationRepository)
    {
        _userRepository = userRepository;
        _moderationRepository = moderationRepository;
    }

    /// <inheritdoc/>
    /// <returns><c>true</c> if the user was disabled; <c>false</c> if the user was not found.</returns>
    /// <remarks>
    /// <para>
    /// Returns <c>false</c> if the user does not exist. Otherwise sets
    /// <see cref="User.IsDisabled"/> to <c>true</c> and persists a
    /// <see cref="ModerationAction"/> with type <see cref="ModerationActionTypes.Disable"/>
    /// recording the moderator and reason.
    /// </para>
    /// </remarks>
    public async Task<bool> Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Return false when the target user does not exist (no-op).
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.IsDisabled = true;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Persist an audit trail recording who performed the disable and why.
        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Disable,
            TargetType = TargetTypes.User,
            TargetId = request.UserId,
            ActionedBy = new User { Id = request.ActionedByUserId },
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }
}
