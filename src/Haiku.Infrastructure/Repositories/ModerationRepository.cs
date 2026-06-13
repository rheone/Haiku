using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for moderation actions and user privileges using EF Core.
/// </summary>
public class ModerationRepository : IModerationRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModerationRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public ModerationRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Records a moderation action (such as a warning, suspension, or content removal).
    /// </summary>
    /// <param name="action">The moderation action entity to persist.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveActionAsync(ModerationAction action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(action);
        if (entry.State == EntityState.Detached)
        {
            _db.ModerationActions.Add(action);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all privileges granted to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of privilege records for the user.</returns>
    public async Task<List<UserPrivilege>> GetUserPrivilegesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.UserPrivileges.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks whether a user has a specific privilege.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="privilege">The privilege name to check (e.g. "Moderate", "Admin").</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the user has the specified privilege; otherwise <c>false</c>.</returns>
    public async Task<bool> HasPrivilegeAsync(Guid userId, string privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.UserPrivileges.AnyAsync(p => p.UserId == userId && p.Privilege == privilege, cancellationToken);
    }

    /// <summary>
    /// Grants a new privilege to a user.
    /// </summary>
    /// <param name="privilege">The privilege entity to grant.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous grant operation.</returns>
    public async Task GrantPrivilegeAsync(UserPrivilege privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.UserPrivileges.Add(privilege);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Revokes a previously granted privilege from a user.
    /// </summary>
    /// <param name="privilege">The privilege entity to revoke.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous revoke operation.</returns>
    public async Task RevokePrivilegeAsync(UserPrivilege privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.UserPrivileges.Remove(privilege);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
