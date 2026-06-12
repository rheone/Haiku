namespace Haiku.Domain.Entities;

public class UserVerificationToken
{
    public virtual Guid Id { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual string Token { get; set; } = string.Empty;
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime ExpiresAt { get; set; }
    public virtual DateTime? UsedAt { get; set; }

    public virtual bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public virtual bool IsUsed => UsedAt.HasValue;
}
