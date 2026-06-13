using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

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

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The user if found; otherwise <c>null</c>.</returns>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The user if found; otherwise <c>null</c>.</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The user if found; otherwise <c>null</c>.</returns>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    /// <summary>
    /// Checks whether a user with the given email address already exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if a user with the specified email exists; otherwise <c>false</c>.</returns>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Checks whether a user with the given username already exists.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if a user with the specified username exists; otherwise <c>false</c>.</returns>
    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    /// <summary>
    /// Persists a new user or saves changes to an existing tracked user.
    /// </summary>
    /// <param name="user">The user entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
        {
            _db.Users.Add(user);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing user entity in the database.
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
        {
            _db.Users.Attach(user);
            entry.State = EntityState.Modified;
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a user entity from the database.
    /// </summary>
    /// <param name="user">The user entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
