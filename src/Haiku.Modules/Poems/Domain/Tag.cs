using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Poems.Domain;

/// <summary>
/// Represents a categorization tag that can be applied to poems.
/// </summary>
/// <remarks>
/// <para>
/// Tags are lightweight, user-created labels for organizing poems by topic, mood,
/// or theme. A poem can have many tags, and a tag can be applied to many poems,
/// linked through the <see cref="PoemTag"/> join entity. Usage counts are tracked
/// daily via <see cref="TagDailyCount"/>.
/// </para>
/// </remarks>
public class Tag
{
    /// <summary>
    /// Gets or sets the primary key, auto-incremented by the database.
    /// </summary>
    /// <value>The tag's unique integer identifier.</value>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the tag, up to 100 characters.
    /// </summary>
    /// <value>The tag display name. Must be unique (enforced by application logic).</value>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
