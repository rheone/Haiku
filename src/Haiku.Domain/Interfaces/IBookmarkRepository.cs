using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

// Repository for the bookmarking feature, keyed on (userId, poemId) composite.

/// <summary>
/// Provides data access for poem bookmarks.
/// </summary>
/// <remarks>
/// <para>Bookmarks let users save poems for later reading. The composite key
/// combines both the user and the poem — a user can bookmark a poem at most once.
/// The bookmark entity itself carries no additional state beyond the key.</para>
/// </remarks>
public interface IBookmarkRepository
{
    /// <summary>
    /// Retrieves a bookmark by its composite user and poem identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching bookmark, or <c>null</c> if no bookmark exists.</returns>
    Task<Bookmark?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default);

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
