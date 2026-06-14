using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a poem (haiku or other poetic form) authored by a user.
/// </summary>
/// <remarks>
/// <para>
/// Poems are the central content entity of the platform. Each poem has a detected
/// <see cref="PoemType"/> and a computed <see cref="TotalSyllables"/> set during
/// creation. Poems support soft-delete via <see cref="DeletedAt"/> and moderation
/// hide via <see cref="IsHidden"/>. Drafts (<see cref="IsDraft"/>) are visible only
/// to their author.
/// </para>
/// </remarks>
[Table("Poems")]
public class Poem
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The poem's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the poem's author.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the author.</value>
    [Required]
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the poem's author.
    /// </summary>
    /// <value>The <see cref="User"/> who authored the poem.</value>
    [ForeignKey(nameof(AuthorId))]
    public User Author { get; set; } = null!;

    /// <summary>
    /// Gets or sets the poem text content, up to 500 characters.
    /// </summary>
    /// <value>The full poem text as a single string with lines separated by newline characters.</value>
    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detected or explicitly assigned poetic form.
    /// </summary>
    /// <value>A <see cref="PoemType"/> value such as Haiku, Tanka, Freeform, etc.</value>
    /// <remarks>
    /// <para>
    /// The column width accommodates all current enum names (max 17 chars for
    /// <c>ButterflyCinquain</c>). If new enum values exceed 30 characters, bump this.
    /// When the <see cref="PoemType"/> enum gains new members, ensure the corresponding
    /// classifier's <c>TypeId</c> follows the kebab-case convention (enum → hyphenated
    /// lowercase) so that <c>ClassifierBuilder.MapToPoemType</c> resolves it automatically.
    /// </para>
    /// </remarks>
    [Column(TypeName = "varchar(30)")]
    public PoemType PoemType { get; set; }

    /// <summary>
    /// Gets or sets the total syllable count across all lines of the poem.
    /// </summary>
    /// <value>The sum of syllable counts per line, computed by <c>SyllableEngine</c> at creation time.</value>
    public int TotalSyllables { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the poem is a draft and visible only to its author.
    /// </summary>
    /// <value><c>true</c> if the poem is a draft; otherwise <c>false</c> (published).</value>
    public bool IsDraft { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the poem is hidden from public view by a moderator.
    /// </summary>
    /// <value><c>true</c> if the poem is hidden; otherwise <c>false</c>.</value>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the poem was created or published.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the poem was soft-deleted, or <c>null</c> if active.
    /// </summary>
    /// <value>The UTC deletion timestamp, or <c>null</c> if the poem has not been deleted.</value>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the poem has been soft-deleted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is computed at runtime from <see cref="DeletedAt"/> and is not persisted.
    /// Soft-deleted poems are excluded from queries by default in the service layer.
    /// </para>
    /// </remarks>
    /// <value><c>true</c> if <see cref="DeletedAt"/> is not null; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsDeleted => DeletedAt.HasValue; // Computed; service layer filters soft-deleted poems by default.
}
