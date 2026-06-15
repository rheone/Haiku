using MicroMediator;

namespace Haiku.Modules.Community.Queries;

/// <summary>
/// Handles retrieval of the net vote score for a poem.
/// </summary>
public class GetVoteScoreQueryHandler : IQueryHandler<GetVoteScoreQuery, int>
{
    private readonly IVoteRepository _voteRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetVoteScoreQueryHandler"/> class.
    /// </summary>
    /// <param name="voteRepository">Repository for computing vote scores.</param>
    public GetVoteScoreQueryHandler(IVoteRepository voteRepository)
    {
        _voteRepository = voteRepository;
    }

    /// <summary>
    /// Computes the net vote score (upvotes minus downvotes) for the specified poem.
    /// </summary>
    /// <param name="request">The query containing the poem identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The net vote score.</returns>
    public async Task<int> Handle(GetVoteScoreQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _voteRepository.GetNetScoreAsync(request.PoemId, cancellationToken);
    }
}
