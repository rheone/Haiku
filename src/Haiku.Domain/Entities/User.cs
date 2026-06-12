using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

public class User
{
    public virtual Guid Id { get; set; }
    public virtual string Email { get; set; } = string.Empty;
    public virtual string Username { get; set; } = string.Empty;
    public virtual string PasswordHash { get; set; } = string.Empty;
    public virtual string DisplayName { get; set; } = string.Empty;
    public virtual string? Bio { get; set; }
    public virtual string? ProfileImageUrl { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime? EmailVerifiedAt { get; set; }
    public virtual bool IsDisabled { get; set; }
    public virtual DateTime? DeletedAt { get; set; }
    public virtual DeletionChoice? DeletionChoice { get; set; }

    public virtual bool IsEmailVerified => EmailVerifiedAt.HasValue;
    public virtual bool IsDeleted => DeletedAt.HasValue;
}
