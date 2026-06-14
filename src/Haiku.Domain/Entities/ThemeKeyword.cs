using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

[Table("ThemeKeywords")]
public class ThemeKeyword
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ThemeId { get; set; }

    [ForeignKey(nameof(ThemeId))]
    public Theme Theme { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Keyword { get; set; } = string.Empty;

    public float Weight { get; set; } = 1.0f;
}
