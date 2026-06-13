using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a tag that can be applied to poems for categorization.
/// </summary>
public class Tag
{
    /// <summary>
    /// Gets or sets the unique identifier for the tag.
    /// </summary>
    /// <value>The unique identifier for the tag.</value>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the tag.
    /// </summary>
    /// <value>The display name of the tag.</value>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
