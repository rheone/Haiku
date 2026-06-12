using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IModerationRepository
{
    Task SaveActionAsync(ModerationAction action);
    Task<List<UserPrivilege>> GetUserPrivilegesAsync(Guid userId);
    Task<bool> HasPrivilegeAsync(Guid userId, string privilege);
    Task GrantPrivilegeAsync(UserPrivilege privilege);
    Task RevokePrivilegeAsync(UserPrivilege privilege);
}
