using MicroMediator;

namespace Haiku.Services.Slices.Votes;

/// <summary>
/// Query to retrieve the net vote score (upvotes minus downvotes) for a poem.
/// </summary>
/// <param name="PoemId">The identifier of the poem whose score to retrieve.</param>
public record GetVoteScoreQuery(Guid PoemId) : IQuery<int>;
