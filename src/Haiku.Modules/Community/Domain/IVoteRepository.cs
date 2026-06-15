namespace Haiku.Modules.Community.Domain;

// Both upvote and downvote are stored as a single Vote entity with a boolean
// direction. GetNetScoreAsync aggregates the difference.

/// <summary>
/// Provides data access for upvote and downvote interactions on poems.
/// </summary>
/// <remarks>
/// <para>Votes provide community-driven feedback on poems. The <c>Vote</c> entity
/// stores whether the vote is an upvote or downvote via a boolean flag. The
/// <see cref="GetNetScoreAsync"/> method computes the aggregate score (upvotes minus
/// downvotes). A user may vote on a poem at most once; changing a vote replaces the
/// existing record.</para>
/// </remarks>
public interface IVoteRepository
{
    /// <summary>
    /// Retrieves a vote by its composite user and poem identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the voting user.</param>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching vote, or <c>null</c> if none exists.</returns>
    Task<Vote?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default);

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
    /// Computes the net score (upvotes minus downvotes) for a poem.
    /// </summary>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The net vote score.</returns>
    Task<int> GetNetScoreAsync(Guid poemId, CancellationToken cancellationToken = default);
}
