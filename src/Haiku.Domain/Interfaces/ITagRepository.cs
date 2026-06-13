using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides data access for poem tags.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Looks up a tag by its display name.
    /// </summary>
    /// <param name="name">The tag name to search for.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching tag, or <c>null</c> if not found.</returns>
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an existing tag by name or creates a new one if none exists.
    /// </summary>
    /// <param name="name">The tag name to find or create.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The existing or newly created tag.</returns>
    Task<Tag> GetOrCreateAsync(string name, CancellationToken cancellationToken = default);
}
