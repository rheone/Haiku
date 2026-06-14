using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a granted authorization privilege that permits a user to perform specific moderator actions.
/// </summary>
/// <remarks>
/// <para>
/// Privileges are fine-grained permissions assigned by other moderators. They are not roles;
/// each privilege corresponds to a single action category defined in <see cref="Enums.PrivilegeNames"/>
/// (e.g., <c>moderate_poems</c>, <c>manage_dictionary</c>). A user may hold multiple privileges.
/// The <see cref="GrantedBy"/> and <see cref="GrantedAt"/> fields provide an audit trail.
/// </para>
/// </remarks>
[Table("UserPrivileges")]
public class UserPrivilege
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The privilege record's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user granted this privilege.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the privileged user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the privileged user.
    /// </summary>
    /// <value>The <see cref="User"/> who holds this privilege.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the privilege identifier string, up to 50 characters.
    /// </summary>
    /// <value>One of the values defined in <see cref="Enums.PrivilegeNames"/> (e.g., <c>moderate_poems</c>, <c>manage_dictionary</c>).</value>
    [Required]
    [MaxLength(50)]
    public string Privilege { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the moderator who granted this privilege.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the granting moderator.</value>
    [Required]
    public Guid GrantedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the granting moderator.
    /// </summary>
    /// <value>The <see cref="User"/> who granted the privilege.</value>
    [ForeignKey(nameof(GrantedByUserId))]
    public User GrantedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the privilege was granted.
    /// </summary>
    /// <value>The UTC grant timestamp.</value>
    public DateTime GrantedAt { get; set; }
}
