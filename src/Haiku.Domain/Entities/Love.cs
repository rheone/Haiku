using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a "love" reaction, a lightweight positive signal from a user to a poem.
/// </summary>
/// <remarks>
/// <para>
/// Unlike <see cref="Vote"/> which supports both upvote and downvote directions,
/// love is unambiguously positive. A user may love a poem at most once; a unique
/// constraint on (UserId, PoemId) enforces this. Love counts are displayed publicly
/// on poems.
/// </para>
/// </remarks>
public class Love
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The love record's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the loved poem.
    /// </summary>
    /// <value>The <see cref="Poem"/> identifier of the loved poem.</value>
    [Required]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the loved poem.
    /// </summary>
    /// <value>The <see cref="Poem"/> being loved.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key to the user expressing love.
    /// </summary>
    /// <value>The <see cref="User"/> identifier expressing love.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user who expressed love.
    /// </summary>
    /// <value>The <see cref="User"/> who expressed love for the poem.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the love was expressed.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
