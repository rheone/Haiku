using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a token used to verify a user's email address during registration.
/// </summary>
[Table("UserVerificationTokens")]
public class UserVerificationToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the verification token.
    /// </summary>
    /// <value>The unique identifier for the verification token.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user to verify.
    /// </summary>
    /// <value>The unique identifier of the user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with this verification token.
    /// </summary>
    /// <value>The <see cref="User"/> to verify.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the cryptographic token string sent to the user's email.
    /// </summary>
    /// <value>The verification token string.</value>
    [Required]
    [MaxLength(128)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the token was created.
    /// </summary>
    /// <value>The token creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp after which the token is no longer valid.
    /// </summary>
    /// <value>The token expiration timestamp.</value>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the token was used, or <c>null</c> if still pending.
    /// </summary>
    /// <value>The usage timestamp, or <c>null</c>.</value>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the token has expired based on the current UTC time.
    /// </summary>
    /// <value><c>true</c> if the token is expired; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Gets a value indicating whether the token has already been used.
    /// </summary>
    /// <value><c>true</c> if the token has been used; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsUsed => UsedAt.HasValue;
}
