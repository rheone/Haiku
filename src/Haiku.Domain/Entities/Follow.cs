using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a follow relationship where one user subscribes to another user's activity.
/// </summary>
public class Follow
{
    /// <summary>
    /// Gets or sets the unique identifier for the follow relationship.
    /// </summary>
    /// <value>The unique identifier for the follow relationship.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who is following.
    /// </summary>
    /// <value>The unique identifier of the follower.</value>
    [Required]
    public Guid FollowerId { get; set; }

    /// <summary>
    /// Gets or sets the user who initiated the follow.
    /// </summary>
    /// <value>The <see cref="User"/> who is following.</value>
    [ForeignKey(nameof(FollowerId))]
    public User Follower { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the user being followed.
    /// </summary>
    /// <value>The unique identifier of the followee.</value>
    [Required]
    public Guid FolloweeId { get; set; }

    /// <summary>
    /// Gets or sets the user being followed.
    /// </summary>
    /// <value>The <see cref="User"/> being followed.</value>
    [ForeignKey(nameof(FolloweeId))]
    public User Followee { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the follow relationship was created.
    /// </summary>
    /// <value>The follow creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
