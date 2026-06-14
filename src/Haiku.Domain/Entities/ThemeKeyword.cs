using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a keyword-weight pair used for automatic theme matching based on content.
/// </summary>
/// <remarks>
/// <para>
/// Themes are associated with multiple keyword-weight pairs. When poem content matches a keyword,
/// the weight influences the confidence score for automatically suggesting or applying the
/// corresponding <see cref="Theme"/>. A higher weight indicates a stronger thematic signal.
/// </para>
/// </remarks>
[Table("ThemeKeywords")]
public class ThemeKeyword
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The keyword record's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the parent theme.
    /// </summary>
    /// <value>The <see cref="Theme"/> identifier.</value>
    [Required]
    public Guid ThemeId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the parent theme.
    /// </summary>
    /// <value>The <see cref="Theme"/> this keyword belongs to.</value>
    [ForeignKey(nameof(ThemeId))]
    public Theme Theme { get; set; } = null!;

    /// <summary>
    /// Gets or sets the keyword text used for content matching.
    /// </summary>
    /// <value>The keyword string, up to 100 characters.</value>
    [Required]
    [MaxLength(100)]
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the matching weight for this keyword, defaults to 1.0.
    /// </summary>
    /// <value>A float value where higher numbers indicate stronger theme affinity.</value>
    public float Weight { get; set; } = 1.0f;
}
