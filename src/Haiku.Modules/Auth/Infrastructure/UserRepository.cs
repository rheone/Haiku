using Microsoft.EntityFrameworkCore;

namespace Haiku.Modules.Auth.Infrastructure;

/// <summary>
/// Persistence store for <see cref="User"/> entities using EF Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public UserRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(user);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.Users.Add(user);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(user);
        // Attach a detached entity to the change tracker and mark it as Modified
        // so EF Core sends an UPDATE for all properties, even when the entity was
        // not retrieved by this DbContext instance.
        if (entry.State == EntityState.Detached)
        {
            _db.Users.Attach(user);
            entry.State = EntityState.Modified;
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
