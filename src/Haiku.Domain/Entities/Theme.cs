using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

[Table("Themes")]
public class Theme
{
    [Key]
    public Guid ThemeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft";

    [MaxLength(50)]
    public string IconFA { get; set; } = "fa-pen-nib";

    public bool HasAnimation { get; set; }

    [MaxLength(50)]
    public string? AnimationKey { get; set; }

    [MaxLength(20)]
    public string? AnimationIntensity { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string LightPaletteJson { get; set; } = "{}";

    [Column(TypeName = "nvarchar(max)")]
    public string DarkPaletteJson { get; set; } = "{}";

    [MaxLength(9)]
    public string? CardTintLight { get; set; }

    [MaxLength(9)]
    public string? CardTintDark { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<ThemeKeyword> Keywords { get; set; } = new List<ThemeKeyword>();
}
