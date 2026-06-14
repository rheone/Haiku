using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

// Tags are shared across poems; GetOrCreateAsync ensures a single canonical
// Tag row per name to avoid duplicates.

/// <summary>
/// Provides data access for poem tags.
/// </summary>
/// <remarks>
/// <para>Tags are reusable labels shared across poems. The <see cref="GetOrCreateAsync"/>
/// method implements a find-or-create pattern that ensures exactly one canonical
/// <c>Tag</c> entity exists per tag name, preventing duplicate tags from being created
/// when multiple authors use the same label.</para>
/// </remarks>
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
