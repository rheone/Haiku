using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for <see cref="Tag"/> entities using EF Core.
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public TagRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a tag by its name.
    /// </summary>
    /// <param name="name">The case-sensitive tag name.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The tag if found; otherwise <c>null</c>.</returns>
    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    /// <summary>
    /// Retrieves an existing tag by name or creates a new one if none exists.
    /// Handles concurrent creation via a race-condition catch of <see cref="DbUpdateException"/>.
    /// </summary>
    /// <param name="name">The tag name to find or create.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The existing or newly created tag.</returns>
    public async Task<Tag> GetOrCreateAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tag = await GetByNameAsync(name, cancellationToken);
        if (tag != null)
        {
            return tag;
        }

        tag = new Tag { Name = name };
        _db.Tags.Add(tag);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException) when (_db.Tags.Any(t => t.Name == name))
        {
            return (await _db.Tags.FirstAsync(t => t.Name == name, cancellationToken))!;
        }

        return tag;
    }
}
