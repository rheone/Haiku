using MicroMediator;

namespace Haiku.Modules.Community.Commands;

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

        // Only upvote (1) and downvote (-1) are accepted. Any other value is silently rejected.
        if (request.Value is not (1 or -1))
        {
            return false;
        }

        var existing = await _voteRepository.GetByUserAndPoemAsync(request.UserId, request.PoemId, cancellationToken);
        if (existing != null)
        {
            // Same vote from the same user on the same poem is a no-op (idempotency).
            if (existing.Value == request.Value)
            {
                return false;
            }

            // Different value means a vote flip: update the existing record instead of creating a new one.
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
