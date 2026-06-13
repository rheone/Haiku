using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides data access for poem bookmarks.
/// </summary>
public interface IBookmarkRepository
{
    /// <summary>
    /// Retrieves a bookmark by its composite user and haiku identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="haikuId">The unique identifier of the haiku poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching bookmark, or <c>null</c> if no bookmark exists.</returns>
    Task<Bookmark?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new or modified bookmark.
    /// </summary>
    /// <param name="bookmark">The bookmark entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(Bookmark bookmark, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a bookmark from the data store.
    /// </summary>
    /// <param name="bookmark">The bookmark entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
}
