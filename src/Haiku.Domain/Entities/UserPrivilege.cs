namespace Haiku.Domain.Entities;

public class UserPrivilege
{
    public virtual Guid Id { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual string Privilege { get; set; } = string.Empty;
    public virtual User GrantedBy { get; set; } = null!;
    public virtual DateTime GrantedAt { get; set; }
}
