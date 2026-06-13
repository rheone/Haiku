using MicroMediator;

namespace Haiku.Services.Slices.Votes;

/// <summary>
/// Command to cast or change a vote on a poem. Vote value must be 1 (upvote) or -1 (downvote).
/// </summary>
/// <param name="PoemId">The identifier of the poem being voted on.</param>
/// <param name="UserId">The identifier of the user casting the vote.</param>
/// <param name="Value">The vote value: 1 for upvote, -1 for downvote. Any other value is rejected by the handler.</param>
public record CastVoteCommand(Guid PoemId, Guid UserId, sbyte Value) : ICommand<bool>;
