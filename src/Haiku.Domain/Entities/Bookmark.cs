using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a poem saved as a bookmark by a user for later reference.
/// </summary>
public class Bookmark
{
    /// <summary>
    /// Gets or sets the unique identifier for the bookmark.
    /// </summary>
    /// <value>The unique identifier for the bookmark.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who bookmarked the poem.
    /// </summary>
    /// <value>The unique identifier of the bookmarking user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user who created the bookmark.
    /// </summary>
    /// <value>The <see cref="User"/> who created the bookmark.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the bookmarked poem.
    /// </summary>
    /// <value>The unique identifier of the bookmarked poem.</value>
    [Required]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the bookmarked poem.
    /// </summary>
    /// <value>The <see cref="Entities.Poem"/> that was bookmarked.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the bookmark was created.
    /// </summary>
    /// <value>The bookmark creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
