using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;

namespace Haiku.Services.Moderation;

/// <summary>
/// Provides moderation actions: hiding or unhiding poems, disabling or reinstating users, and permission checks.
/// </summary>
public class ModerationService
{
    private readonly IModerationRepository _moderationRepository;
    private readonly IPoemRepository _poemRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModerationService"/> class.
    /// </summary>
    /// <param name="moderationRepository">Repository for moderation action audit records.</param>
    /// <param name="poemRepository">Repository for poem entities.</param>
    /// <param name="userRepository">Repository for user entities.</param>
    public ModerationService(
        IModerationRepository moderationRepository,
        IPoemRepository poemRepository,
        IUserRepository userRepository
    )
    {
        _moderationRepository = moderationRepository;
        _poemRepository = poemRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Hides a poem so it is no longer publicly visible. Creates an audit record of the action.
    /// </summary>
    /// <param name="poemId">The ID of the poem to hide.</param>
    /// <param name="actionedByUserId">The ID of the moderator performing the action.</param>
    /// <param name="reason">The reason for hiding the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the poem was found and hidden; <c>false</c> if the poem does not exist.</returns>
    public async Task<bool> HidePoemAsync(
        Guid poemId,
        Guid actionedByUserId,
        string reason,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var poem = await _poemRepository.GetByIdAsync(poemId, cancellationToken);
        if (poem == null)
        {
            return false;
        }

        poem.IsHidden = true;
        await _poemRepository.SaveAsync(poem, cancellationToken);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Hide,
            TargetType = TargetTypes.Poem,
            TargetId = poemId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }

    /// <summary>
    /// Restores a previously hidden poem to public visibility. Creates an audit record of the action.
    /// </summary>
    /// <param name="poemId">The ID of the poem to unhide.</param>
    /// <param name="actionedByUserId">The ID of the moderator performing the action.</param>
    /// <param name="reason">The reason for unhiding the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the poem was found and unhidden; <c>false</c> if it does not exist.</returns>
    public async Task<bool> UnhidePoemAsync(
        Guid poemId,
        Guid actionedByUserId,
        string reason,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var poem = await _poemRepository.GetByIdAsync(poemId, cancellationToken);
        if (poem == null)
        {
            return false;
        }

        poem.IsHidden = false;
        await _poemRepository.SaveAsync(poem, cancellationToken);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Unhide,
            TargetType = TargetTypes.Poem,
            TargetId = poemId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }

    /// <summary>
    /// Disables a user account, preventing login. Creates an audit record of the action.
    /// </summary>
    /// <param name="userId">The ID of the user to disable.</param>
    /// <param name="actionedByUserId">The ID of the moderator performing the action.</param>
    /// <param name="reason">The reason for disabling the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the user was found and disabled; <c>false</c> if the user does not exist.</returns>
    public async Task<bool> DisableUserAsync(
        Guid userId,
        Guid actionedByUserId,
        string reason,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.IsDisabled = true;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Disable,
            TargetType = TargetTypes.User,
            TargetId = userId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }

    /// <summary>
    /// Reinstates a previously disabled user account, restoring login access. Creates an audit record of the action.
    /// </summary>
    /// <param name="userId">The ID of the user to reinstate.</param>
    /// <param name="actionedByUserId">The ID of the moderator performing the action.</param>
    /// <param name="reason">The reason for reinstating the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the user was found and reinstated; <c>false</c> if the user does not exist.</returns>
    public async Task<bool> ReinstateUserAsync(
        Guid userId,
        Guid actionedByUserId,
        string reason,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.IsDisabled = false;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Reinstate,
            TargetType = TargetTypes.User,
            TargetId = userId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
        };
        await _moderationRepository.SaveActionAsync(action, cancellationToken);
        return true;
    }

    /// <summary>
    /// Checks whether a user has a specific moderation privilege.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <param name="privilege">The privilege string to verify.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the user has the specified privilege.</returns>
    public async Task<bool> HasPrivilegeAsync(Guid userId, string privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _moderationRepository.HasPrivilegeAsync(userId, privilege, cancellationToken);
    }
}
