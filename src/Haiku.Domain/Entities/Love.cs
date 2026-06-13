using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a "love" appreciation interaction from a user to a poem.
/// </summary>
public class Love
{
    /// <summary>
    /// Gets or sets the unique identifier for the love record.
    /// </summary>
    /// <value>The unique identifier for the love record.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the poem being loved.
    /// </summary>
    /// <value>The unique identifier of the poem.</value>
    [Required]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the poem being loved.
    /// </summary>
    /// <value>The <see cref="Entities.Poem"/> being loved.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the user expressing love.
    /// </summary>
    /// <value>The unique identifier of the user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user who expressed love for the poem.
    /// </summary>
    /// <value>The <see cref="User"/> who expressed love.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the love was expressed.
    /// </summary>
    /// <value>The love creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
