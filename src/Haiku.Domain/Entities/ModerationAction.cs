using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents an immutable audit record of a moderation action taken against content or a user.
/// </summary>
/// <remarks>
/// <para>
/// This is an append-only log. Records are never modified or deleted after creation.
/// The <see cref="ActionType"/> and <see cref="TargetType"/> values map to the string
/// constants defined in <see cref="Enums.ModerationActionTypes"/> and <see cref="Enums.TargetTypes"/>.
/// A moderator with the corresponding privilege (e.g., <c>moderate_poems</c>) performs the action.
/// </para>
/// </remarks>
[Table("ModerationActions")]
public class ModerationAction
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The action record's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of action taken, stored as a string discriminator.
    /// </summary>
    /// <value>One of <see cref="Enums.ModerationActionTypes"/>: Hide, Unhide, Disable, or Reinstate.</value>
    [Required]
    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of target entity the action was applied to, stored as a string discriminator.
    /// </summary>
    /// <value>One of <see cref="Enums.TargetTypes"/>: Poem, User, DictionaryWord, or DictionarySuggestion.</value>
    [Required]
    [MaxLength(20)]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the entity that was the target of this action.
    /// </summary>
    /// <value>The PK of the target entity (e.g., Poem.Id, User.Id).</value>
    public Guid TargetId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the moderator who performed the action.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the acting moderator.</value>
    [Required]
    public Guid ActionedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the acting moderator.
    /// </summary>
    /// <value>The <see cref="User"/> who performed the action.</value>
    [ForeignKey(nameof(ActionedByUserId))]
    public User ActionedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the human-readable reason for the moderation action, up to 500 characters.
    /// </summary>
    /// <value>The moderation reason text.</value>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the action was recorded.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
