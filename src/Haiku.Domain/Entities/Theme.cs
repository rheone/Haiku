using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a visual UI theme with configurable color palettes, animations, and keyword-based matching rules.
/// </summary>
/// <remarks>
/// <para>
/// Themes control the visual appearance of the platform. Each theme defines separate light and dark color
/// palettes stored as JSON, optional CSS animation effects, and a set of <see cref="ThemeKeyword"/> entries
/// that enable automatic theme matching based on poem content. The <see cref="Status"/> property controls
/// visibility: only published themes are available for use.
/// </para>
/// </remarks>
[Table("Themes")]
public class Theme
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The theme's unique database identifier.</value>
    [Key]
    public Guid ThemeId { get; set; }

    /// <summary>
    /// Gets or sets the unique key identifier used to reference this theme in code and URLs.
    /// </summary>
    /// <value>A unique string key (e.g., "ocean", "sunset").</value>
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable display name shown in theme selection UI.
    /// </summary>
    /// <value>The theme display name (e.g., "Ocean Depths", "Sunset Glow").</value>
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the publication status of this theme.
    /// Defaults to "Draft".
    /// </summary>
    /// <value>"Draft" while in development, "Published" when available to users.</value>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Gets or sets the Font Awesome icon class for theme thumbnails and previews.
    /// </summary>
    /// <value>A Font Awesome icon class (e.g., "fa-pen-nib").</value>
    [MaxLength(50)]
    public string IconFA { get; set; } = "fa-pen-nib";

    /// <summary>
    /// Gets or sets a value indicating whether this theme applies CSS animations.
    /// </summary>
    /// <value><c>true</c> if animations are enabled; otherwise <c>false</c>.</value>
    public bool HasAnimation { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the CSS animation preset to apply.
    /// </summary>
    /// <value>The animation key (e.g., "fade", "slide"), or <c>null</c> if no animation is set.</value>
    [MaxLength(50)]
    public string? AnimationKey { get; set; }

    /// <summary>
    /// Gets or sets the intensity level for the CSS animation.
    /// </summary>
    /// <value>The animation intensity (e.g., "subtle", "medium", "pronounced"), or <c>null</c> if not specified.</value>
    [MaxLength(20)]
    public string? AnimationIntensity { get; set; }

    /// <summary>
    /// Gets or sets the light mode color palette as a JSON object.
    /// </summary>
    /// <value>A JSON string defining CSS custom property values for light mode, defaults to an empty object.</value>
    [Column(TypeName = "nvarchar(max)")]
    public string LightPaletteJson { get; set; } = "{}";

    /// <summary>
    /// Gets or sets the dark mode color palette as a JSON object.
    /// </summary>
    /// <value>A JSON string defining CSS custom property values for dark mode, defaults to an empty object.</value>
    [Column(TypeName = "nvarchar(max)")]
    public string DarkPaletteJson { get; set; } = "{}";

    /// <summary>
    /// Gets or sets the card accent tint color for light mode, stored as a hex color with optional alpha.
    /// </summary>
    /// <value>A hex color string (e.g., "#FF8800" or "#FF880080"), up to 9 characters, or <c>null</c> if not set.</value>
    [MaxLength(9)]
    public string? CardTintLight { get; set; }

    /// <summary>
    /// Gets or sets the card accent tint color for dark mode, stored as a hex color with optional alpha.
    /// </summary>
    /// <value>A hex color string (e.g., "#FF8800" or "#FF880080"), up to 9 characters, or <c>null</c> if not set.</value>
    [MaxLength(9)]
    public string? CardTintDark { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the theme was created.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the theme was last modified.
    /// </summary>
    /// <value>The UTC last-modified timestamp.</value>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of keyword-weight pairs for automatic theme matching.
    /// </summary>
    /// <value>The <see cref="ThemeKeyword"/> entries linked to this theme.</value>
    public ICollection<ThemeKeyword> Keywords { get; set; } = new List<ThemeKeyword>();
}
