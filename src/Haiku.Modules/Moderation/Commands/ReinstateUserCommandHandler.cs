using MicroMediator;

namespace Haiku.Modules.Moderation.Commands;

/// <summary>
/// Handles reinstating a disabled user account and logging the moderation action for audit purposes.
/// </summary>
public class ReinstateUserCommandHandler : ICommandHandler<ReinstateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IModerationRepository _moderationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReinstateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository for data access.</param>
    /// <param name="moderationRepository">The moderation repository for audit logging.</param>
    public ReinstateUserCommandHandler(IUserRepository userRepository, IModerationRepository moderationRepository)
    {
        _userRepository = userRepository;
        _moderationRepository = moderationRepository;
    }

    /// <inheritdoc/>
    /// <returns><c>true</c> if the user was reinstated; <c>false</c> if the user was not found.</returns>
    /// <remarks>
    /// <para>
    /// Returns <c>false</c> if the user does not exist. Otherwise sets
    /// <see cref="User.IsDisabled"/> to <c>false</c> and persists a
    /// <see cref="ModerationAction"/> with type <see cref="ModerationActionTypes.Reinstate"/>
    /// recording the moderator and reason.
    /// </para>
    /// </remarks>
    public async Task<bool> Handle(ReinstateUserCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Return false when the target user does not exist (no-op).
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.IsDisabled = false;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Persist an audit trail recording who reinstated the user and why.
        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Reinstate,
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
