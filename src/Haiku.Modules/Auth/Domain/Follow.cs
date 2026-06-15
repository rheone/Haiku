using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Auth.Domain;

/// <summary>
/// Represents a unidirectional follow relationship where one user subscribes to another's activity.
/// </summary>
/// <remarks>
/// <para>
/// Follows are not reciprocal: user A can follow user B without B following A. When a user
/// follows another, the follower sees the followee's published poems in their activity feed.
/// A unique constraint on (FollowerId, FolloweeId) prevents duplicate follows.
/// </para>
/// </remarks>
public class Follow
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The follow relationship's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user who initiated the follow.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the follower.</value>
    [Required]
    public Guid FollowerId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the follower.
    /// </summary>
    /// <value>The <see cref="User"/> who is following.</value>
    [ForeignKey(nameof(FollowerId))]
    public User Follower { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key to the user being followed.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the followee.</value>
    [Required]
    public Guid FolloweeId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user being followed.
    /// </summary>
    /// <value>The <see cref="User"/> being followed.</value>
    [ForeignKey(nameof(FolloweeId))]
    public User Followee { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the follow was created.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
