using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents an audit record of a moderation action taken against content or a user.
/// </summary>
[Table("ModerationActions")]
public class ModerationAction
{
    /// <summary>
    /// Gets or sets the unique identifier for the moderation action.
    /// </summary>
    /// <value>The unique identifier for the moderation action.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of action taken (e.g., Hide, Disable). See <see cref="Enums.ModerationActionTypes"/>.
    /// </summary>
    /// <value>The moderation action type string.</value>
    [Required]
    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of target the action was applied to (e.g., Haiku, User). See <see cref="Enums.TargetTypes"/>.
    /// </summary>
    /// <value>The target type string.</value>
    [Required]
    [MaxLength(20)]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the target entity.
    /// </summary>
    /// <value>The unique identifier of the target.</value>
    public Guid TargetId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the moderator who performed the action.
    /// </summary>
    /// <value>The unique identifier of the actioning moderator.</value>
    [Required]
    public Guid ActionedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the moderator who performed the action.
    /// </summary>
    /// <value>The <see cref="User"/> who performed the action.</value>
    [ForeignKey(nameof(ActionedByUserId))]
    public User ActionedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the reason provided for the moderation action.
    /// </summary>
    /// <value>The moderation reason text.</value>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the moderation action was recorded.
    /// </summary>
    /// <value>The action creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
