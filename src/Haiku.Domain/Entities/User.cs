using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a registered user account on the haiku platform.
/// </summary>
/// <remarks>
/// <para>
/// Users are the primary actors on the platform. Each user can author poems, follow other
/// users, vote and love poems, bookmark favorites, and manage a profile. Accounts support
/// soft-delete (<see cref="DeletedAt"/>) with a configurable <see cref="DeletionChoice"/>
/// for how the user's content is handled. Moderators can disable accounts via <see cref="IsDisabled"/>.
/// Email verification is tracked through <see cref="EmailVerifiedAt"/>. Privileges for
/// moderation actions are stored separately in <see cref="UserPrivilege"/>.
/// </para>
/// </remarks>
public class User
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The user's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address used for authentication and notifications, up to 320 characters.
    /// </summary>
    /// <value>The email address. Must be unique and confirmed via <see cref="EmailVerifiedAt"/> before full access is granted.</value>
    [Required]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique public-facing username, up to 50 characters.
    /// </summary>
    /// <value>The public username displayed on the user's profile and alongside their poems.</value>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bcrypt hash of the user's password, up to 200 characters.
    /// </summary>
    /// <value>The bcrypt hash. The raw password is never stored or logged.</value>
    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional display name shown on the user's profile, up to 100 characters.
    /// </summary>
    /// <value>The display name, or <see cref="string.Empty"/> if not set.</value>
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional biography text displayed on the user's profile, up to 1,000 characters.
    /// </summary>
    /// <value>The biography text, or <c>null</c> if not provided.</value>
    [MaxLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the URL to the user's profile image, up to 500 characters.
    /// </summary>
    /// <value>A <see cref="Uri"/> pointing to the avatar image, or <c>null</c> if no image is set.</value>
    [MaxLength(500)]
    public Uri? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the account was created.
    /// </summary>
    /// <value>The UTC account creation timestamp.</value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the user's email was verified, or <c>null</c> if not yet verified.
    /// </summary>
    /// <value>The UTC email verification timestamp, or <c>null</c> if pending verification.</value>
    public DateTime? EmailVerifiedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the account has been disabled by a moderator.
    /// </summary>
    /// <value><c>true</c> if the account is disabled and login is blocked; otherwise <c>false</c>.</value>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the account was soft-deleted, or <c>null</c> if active.
    /// </summary>
    /// <value>The UTC deletion timestamp, or <c>null</c> if the account has not been deleted.</value>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the user's preference for how their authored content is handled upon account deletion.
    /// </summary>
    /// <value>A <see cref="Enums.DeletionChoice"/>: <c>Anonymize</c> to keep content with an anonymous marker, or <c>Remove</c> to permanently delete it.</value>
    [Column(TypeName = "varchar(10)")]
    public DeletionChoice? DeletionChoice { get; set; }

    /// <summary>
    /// Gets a value indicating whether the user's email address has been confirmed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is computed at runtime from <see cref="EmailVerifiedAt"/> and is not persisted.
    /// Unverified users may have restricted access to certain platform features.
    /// </para>
    /// </remarks>
    /// <value><c>true</c> if <see cref="EmailVerifiedAt"/> has a value; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsEmailVerified => EmailVerifiedAt.HasValue; // Computed; unverified users may have restricted access.

    /// <summary>
    /// Gets a value indicating whether the account has been soft-deleted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is computed at runtime from <see cref="DeletedAt"/> and is not persisted.
    /// Soft-deleted accounts are excluded from queries by default.
    /// </para>
    /// </remarks>
    /// <value><c>true</c> if <see cref="DeletedAt"/> is not null; otherwise <c>false</c>.</value>
    [NotMapped]
    public bool IsDeleted => DeletedAt.HasValue; // Computed; service layer excludes soft-deleted accounts.
}
