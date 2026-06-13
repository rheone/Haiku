using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Votes;

/// <summary>
/// Handles vote casting with deduplication and vote-flip semantics. Only values of 1 (upvote) and -1 (downvote) are accepted.
/// </summary>
public class CastVoteCommandHandler : ICommandHandler<CastVoteCommand, bool>
{
    private readonly IVoteRepository _voteRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastVoteCommandHandler"/> class.
    /// </summary>
    /// <param name="voteRepository">Repository for loading and saving <see cref="Vote"/> entities.</param>
    public CastVoteCommandHandler(IVoteRepository voteRepository)
    {
        _voteRepository = voteRepository;
    }

    /// <summary>
    /// Casts a vote or flips an existing vote. Returns <c>false</c> when the value is out of range
    /// or the user has already cast the same vote on this poem (no-op).
    /// </summary>
    /// <param name="request">The command containing poem, user, and vote value.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the vote was recorded or changed; <c>false</c> if rejected or duplicate.</returns>
    public async Task<bool> Handle(CastVoteCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Value is not (1 or -1))
        {
            return false;
        }

        var existing = await _voteRepository.GetByUserAndHaikuAsync(request.UserId, request.PoemId, cancellationToken);
        if (existing != null)
        {
            if (existing.Value == request.Value)
            {
                return false;
            }

            existing.Value = request.Value;
            await _voteRepository.SaveAsync(existing, cancellationToken);
            return true;
        }

        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            Poem = new Poem { Id = request.PoemId },
            User = new User { Id = request.UserId },
            Value = request.Value,
            CreatedAt = DateTime.UtcNow,
        };

        await _voteRepository.SaveAsync(vote, cancellationToken);
        return true;
    }
}
