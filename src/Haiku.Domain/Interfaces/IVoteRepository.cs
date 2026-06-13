using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides data access for upvote and downvote interactions on haiku poems.
/// </summary>
public interface IVoteRepository
{
    /// <summary>
    /// Retrieves a vote by its composite user and haiku identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the voting user.</param>
    /// <param name="haikuId">The unique identifier of the haiku poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching vote, or <c>null</c> if none exists.</returns>
    Task<Vote?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new or modified vote.
    /// </summary>
    /// <param name="vote">The vote entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(Vote vote, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a vote from the data store.
    /// </summary>
    /// <param name="vote">The vote entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Vote vote, CancellationToken cancellationToken = default);

    /// <summary>
    /// Computes the net score (upvotes minus downvotes) for a haiku.
    /// </summary>
    /// <param name="haikuId">The unique identifier of the haiku poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The net vote score.</returns>
    Task<int> GetNetScoreAsync(Guid haikuId, CancellationToken cancellationToken = default);
}
