using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;

namespace Haiku.Services.Moderation;

public class ModerationService
{
    private readonly IModerationRepository _moderationRepository;
    private readonly IHaikuRepository _haikuRepository;
    private readonly IUserRepository _userRepository;

    public ModerationService(
        IModerationRepository moderationRepository,
        IHaikuRepository haikuRepository,
        IUserRepository userRepository)
    {
        _moderationRepository = moderationRepository;
        _haikuRepository = haikuRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> HidePoemAsync(Guid poemId, Guid actionedByUserId, string reason)
    {
        var poem = await _haikuRepository.GetByIdAsync(poemId);
        if (poem == null) return false;

        poem.IsHidden = true;
        await _haikuRepository.SaveAsync(poem);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Hide,
            TargetType = TargetTypes.Haiku,
            TargetId = poemId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };
        await _moderationRepository.SaveActionAsync(action);
        return true;
    }

    public async Task<bool> UnhidePoemAsync(Guid poemId, Guid actionedByUserId, string reason)
    {
        var poem = await _haikuRepository.GetByIdAsync(poemId);
        if (poem == null) return false;

        poem.IsHidden = false;
        await _haikuRepository.SaveAsync(poem);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Unhide,
            TargetType = TargetTypes.Haiku,
            TargetId = poemId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };
        await _moderationRepository.SaveActionAsync(action);
        return true;
    }

    public async Task<bool> DisableUserAsync(Guid userId, Guid actionedByUserId, string reason)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.IsDisabled = true;
        await _userRepository.UpdateAsync(user);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Disable,
            TargetType = TargetTypes.User,
            TargetId = userId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };
        await _moderationRepository.SaveActionAsync(action);
        return true;
    }

    public async Task<bool> ReinstateUserAsync(Guid userId, Guid actionedByUserId, string reason)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.IsDisabled = false;
        await _userRepository.UpdateAsync(user);

        var action = new ModerationAction
        {
            Id = Guid.NewGuid(),
            ActionType = ModerationActionTypes.Reinstate,
            TargetType = TargetTypes.User,
            TargetId = userId,
            ActionedBy = new User { Id = actionedByUserId },
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };
        await _moderationRepository.SaveActionAsync(action);
        return true;
    }

    public async Task<bool> HasPrivilegeAsync(Guid userId, string privilege) =>
        await _moderationRepository.HasPrivilegeAsync(userId, privilege);
}
