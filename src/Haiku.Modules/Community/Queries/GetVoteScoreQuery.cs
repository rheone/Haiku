using MicroMediator;

namespace Haiku.Modules.Community.Queries;

/// <summary>
/// Query to retrieve the net vote score (upvotes minus downvotes) for a poem.
/// </summary>
/// <param name="PoemId">The identifier of the poem whose score to retrieve.</param>
public record GetVoteScoreQuery(Guid PoemId) : IQuery<int>;
