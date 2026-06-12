namespace Haiku.Domain.Entities;

public class Tag
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
}
