using Haiku.Domain.Entities;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides data access for moderation actions and user privilege management.
/// </summary>
public interface IModerationRepository
{
    /// <summary>
    /// Records a moderation action in the audit log.
    /// </summary>
    /// <param name="action">The moderation action to persist.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveActionAsync(ModerationAction action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all privileges assigned to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of privilege entities for the user.</returns>
    Task<List<UserPrivilege>> GetUserPrivilegesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user holds a specific privilege.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="privilege">The privilege name to check (see <see cref="PrivilegeNames"/>).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the user has the privilege; otherwise <c>false</c>.</returns>
    Task<bool> HasPrivilegeAsync(Guid userId, string privilege, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a new privilege to a user.
    /// </summary>
    /// <param name="privilege">The privilege entity to create.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GrantPrivilegeAsync(UserPrivilege privilege, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a privilege from a user.
    /// </summary>
    /// <param name="privilege">The privilege entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RevokePrivilegeAsync(UserPrivilege privilege, CancellationToken cancellationToken = default);
}
