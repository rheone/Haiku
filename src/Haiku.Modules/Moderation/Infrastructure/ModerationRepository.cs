using Microsoft.EntityFrameworkCore;

namespace Haiku.Modules.Moderation.Infrastructure;

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

    /// <inheritdoc/>
    public async Task SaveActionAsync(ModerationAction action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(action);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.ModerationActions.Add(action);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<UserPrivilege>> GetUserPrivilegesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.UserPrivileges.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> HasPrivilegeAsync(Guid userId, string privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.UserPrivileges.AnyAsync(p => p.UserId == userId && p.Privilege == privilege, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task GrantPrivilegeAsync(UserPrivilege privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.UserPrivileges.Add(privilege);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RevokePrivilegeAsync(UserPrivilege privilege, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.UserPrivileges.Remove(privilege);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
