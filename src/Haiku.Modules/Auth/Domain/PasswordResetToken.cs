using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Auth.Domain;

/// <summary>
/// Represents a time-limited cryptographic token that authorizes a password reset.
/// </summary>
/// <remarks>
/// <para>
/// Tokens are single-use: after a successful password reset the token is marked as used
/// via <see cref="UsedAt"/> and subsequent attempts with the same token are rejected.
/// Expired tokens (<see cref="IsExpired"/>) are also rejected regardless of use status.
/// The token string itself is generated cryptographically and hashed before storage.
/// </para>
/// </remarks>
[Table("PasswordResetTokens")]
public class PasswordResetToken
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The token record's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user requesting the reset.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the reset requestor.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user requesting the reset.
    /// </summary>
    /// <value>The <see cref="User"/> who requested the password reset.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the opaque token string sent to the user's email, up to 128 characters.
    /// </summary>
    /// <value>The password reset token string.</value>
    [Required]
    [MaxLength(128)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the token was created.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) after which the token is no longer valid.
    /// </summary>
    /// <value>The UTC expiration timestamp.</value>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the token was consumed, or <c>null</c> if still active.
    /// </summary>
    /// <value>The UTC usage timestamp, or <c>null</c> if the token has not been used.</value>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the token has expired.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is computed at runtime and is not persisted to the database.
    /// Comparison uses the current UTC clock, so token validity depends on the server clock.
    /// </para>
    /// </remarks>
    /// <value><c>true</c> if the current UTC time is past <see cref="ExpiresAt"/>; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow > ExpiresAt; // Server-clock dependent; expiry check is runtime-only.

    /// <summary>
    /// Gets a value indicating whether the token has already been consumed.
    /// </summary>
    /// <value><c>true</c> if <see cref="UsedAt"/> has a value; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsUsed => UsedAt.HasValue; // Once used, the token is permanently invalidated.
}
