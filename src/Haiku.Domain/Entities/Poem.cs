using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a haiku poem authored by a user.
/// </summary>
[Table("Haikus")]
public class Poem
{
    /// <summary>
    /// Gets or sets the unique identifier for the poem.
    /// </summary>
    /// <value>The unique identifier for the poem.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the author.
    /// </summary>
    /// <value>The unique identifier of the author.</value>
    [Required]
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the author of the poem.
    /// </summary>
    /// <value>The <see cref="User"/> who authored the poem.</value>
    [ForeignKey(nameof(AuthorId))]
    public User Author { get; set; } = null!;

    /// <summary>
    /// Gets or sets the poem text content.
    /// </summary>
    /// <value>The poem text content.</value>
    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detected or assigned poetic form.
    /// </summary>
    /// <value>A <see cref="PoemType"/> value indicating the poetic form.</value>
    [Column(TypeName = "varchar(20)")]
    public PoemType PoemType { get; set; }

    /// <summary>
    /// Gets or sets the total syllable count across all lines.
    /// </summary>
    /// <value>The total syllable count.</value>
    public int TotalSyllables { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the poem is a draft and not yet published.
    /// </summary>
    /// <value><c>true</c> if the poem is a draft; otherwise <c>false</c>.</value>
    public bool IsDraft { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the poem is hidden from public view.
    /// </summary>
    /// <value><c>true</c> if the poem is hidden; otherwise <c>false</c>.</value>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the poem was created.
    /// </summary>
    /// <value>The creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the poem was soft-deleted, or <c>null</c> if active.
    /// </summary>
    /// <value>The deletion timestamp, or <c>null</c>.</value>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the poem has been soft-deleted.
    /// </summary>
    /// <value><c>true</c> if the poem is deleted; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsDeleted => DeletedAt.HasValue;
}
