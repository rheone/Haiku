using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

// Standard CRUD access for poem entities. Poems are the central content type
// in the platform — all social features (bookmark, love, vote) revolve around them.

/// <summary>
/// Provides data access for poem entities.
/// </summary>
/// <remarks>
/// <para>Poems are the core content entity on the platform. This repository provides
/// basic CRUD operations. Queries for lists (feed, search, user poems) are handled
/// by feature-specific query handlers rather than this repository, following the
/// CQRS principle of separating read and write concerns.</para>
/// </remarks>
public interface IPoemRepository
{
    /// <summary>
    /// Retrieves a poem by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching poem, or <c>null</c> if not found.</returns>
    Task<Poem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new or modified poem.
    /// </summary>
    /// <param name="poem">The poem entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(Poem poem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a poem from the data store.
    /// </summary>
    /// <param name="poem">The poem entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Poem poem, CancellationToken cancellationToken = default);
}
