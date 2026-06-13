using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents an upvote or downvote cast by a user on a poem.
/// </summary>
public class Vote
{
    /// <summary>
    /// Gets or sets the unique identifier for the vote.
    /// </summary>
    /// <value>The unique identifier for the vote.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the poem being voted on.
    /// </summary>
    /// <value>The unique identifier of the poem.</value>
    [Required]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the poem being voted on.
    /// </summary>
    /// <value>The <see cref="Entities.Poem"/> being voted on.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the voting user.
    /// </summary>
    /// <value>The unique identifier of the voting user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user who cast the vote.
    /// </summary>
    /// <value>The <see cref="User"/> who cast the vote.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the vote value. Positive values represent upvotes; negative values represent downvotes.
    /// </summary>
    /// <value>A signed byte where positive is upvote and negative is downvote.</value>
    public sbyte Value { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the vote was cast.
    /// </summary>
    /// <value>The vote creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
