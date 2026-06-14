namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.Vote"/> entity instantiation and property assignment.
/// Votes are directional: +1 for upvote, -1 for downvote. Unlike <see cref="Haiku.Domain.Entities.Love"/>
/// (exclusively positive), votes allow nuanced community feedback. A user may vote on a
/// given poem only once; subsequent votes replace the previous value.
/// </summary>
public class VoteTests { }
