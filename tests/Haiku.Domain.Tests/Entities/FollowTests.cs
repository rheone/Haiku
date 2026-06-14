namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.Follow"/> entity instantiation and property assignment.
/// Follows are unidirectional subscriptions where a user (follower) receives the followee's
/// published poems in their activity feed. Duplicate follows are prevented by a unique
/// constraint on (FollowerId, FolloweeId).
/// </summary>
public class FollowTests { }
