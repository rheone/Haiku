using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a granted privilege that authorizes a user for a specific moderator action.
/// </summary>
[Table("UserPrivileges")]
public class UserPrivilege
{
    /// <summary>
    /// Gets or sets the unique identifier for the privilege record.
    /// </summary>
    /// <value>The unique identifier for the privilege record.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user granted the privilege.
    /// </summary>
    /// <value>The unique identifier of the privileged user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user granted the privilege.
    /// </summary>
    /// <value>The <see cref="User"/> granted the privilege.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the privilege name (e.g., moderate_poems, manage_dictionary). See <see cref="Enums.PrivilegeNames"/>.
    /// </summary>
    /// <value>The privilege name string.</value>
    [Required]
    [MaxLength(50)]
    public string Privilege { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the moderator who granted the privilege.
    /// </summary>
    /// <value>The unique identifier of the granting moderator.</value>
    [Required]
    public Guid GrantedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the moderator who granted the privilege.
    /// </summary>
    /// <value>The <see cref="User"/> who granted the privilege.</value>
    [ForeignKey(nameof(GrantedByUserId))]
    public User GrantedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the privilege was granted.
    /// </summary>
    /// <value>The privilege grant timestamp.</value>
    public DateTime GrantedAt { get; set; }
}
