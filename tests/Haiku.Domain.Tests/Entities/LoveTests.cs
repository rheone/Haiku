namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.Love"/> entity instantiation and property assignment.
/// Love is an unambiguously positive reaction to a poem, distinct from the directional
/// upvote/downvote semantics of <see cref="Haiku.Domain.Entities.Vote"/>. A user may love
/// a poem at most once, enforced by a unique constraint on (UserId, PoemId).
/// </summary>
public class LoveTests { }
