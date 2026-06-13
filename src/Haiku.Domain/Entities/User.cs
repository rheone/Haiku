using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a registered user account on the platform.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    /// <value>The unique identifier for the user.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address used for authentication and notifications.
    /// </summary>
    /// <value>The email address for the user.</value>
    [Required]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique public-facing username.
    /// </summary>
    /// <value>The public-facing username.</value>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bcrypt hash of the user's password.
    /// </summary>
    /// <value>The bcrypt hash of the password.</value>
    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional display name shown on the user's profile.
    /// </summary>
    /// <value>The optional display name.</value>
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional biography text displayed on the user's profile.
    /// </summary>
    /// <value>The optional biography text.</value>
    [MaxLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the URL to the user's profile image.
    /// </summary>
    /// <value>The URL to the profile image.</value>
    [MaxLength(500)]
    public Uri? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the account was created.
    /// </summary>
    /// <value>The account creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the user's email was verified, or <c>null</c> if not yet verified.
    /// </summary>
    /// <value>The email verification timestamp, or <c>null</c>.</value>
    public DateTime? EmailVerifiedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the account has been disabled by a moderator.
    /// </summary>
    /// <value><c>true</c> if the account is disabled; otherwise <c>false</c>.</value>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the account was soft-deleted, or <c>null</c> if active.
    /// </summary>
    /// <value>The deletion timestamp, or <c>null</c>.</value>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the user's preference for how their content is handled upon account deletion.
    /// </summary>
    /// <value>A <see cref="Enums.DeletionChoice"/> value indicating the user's preference.</value>
    [Column(TypeName = "varchar(10)")]
    public DeletionChoice? DeletionChoice { get; set; }

    /// <summary>
    /// Gets a value indicating whether the user's email address has been confirmed.
    /// </summary>
    /// <value><c>true</c> if the email is verified; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsEmailVerified => EmailVerifiedAt.HasValue;

    /// <summary>
    /// Gets a value indicating whether the account has been soft-deleted.
    /// </summary>
    /// <value><c>true</c> if the account is deleted; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsDeleted => DeletedAt.HasValue;
}
