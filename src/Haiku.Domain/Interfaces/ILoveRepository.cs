using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides data access for haiku appreciation ("love") interactions.
/// </summary>
public interface ILoveRepository
{
    /// <summary>
    /// Retrieves a love record by its composite user and haiku identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="haikuId">The unique identifier of the haiku poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching love record, or <c>null</c> if none exists.</returns>
    Task<Love?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new or modified love record.
    /// </summary>
    /// <param name="love">The love entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(Love love, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a love record from the data store.
    /// </summary>
    /// <param name="love">The love entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Love love, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total number of loves a haiku has received.
    /// </summary>
    /// <param name="haikuId">The unique identifier of the haiku poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The love count.</returns>
    Task<int> GetLoveCountAsync(Guid haikuId, CancellationToken cancellationToken = default);
}
