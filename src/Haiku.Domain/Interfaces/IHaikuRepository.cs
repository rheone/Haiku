using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IHaikuRepository
{
    Task<Poem?> GetByIdAsync(Guid id);
    Task SaveAsync(Poem poem);
    Task DeleteAsync(Poem poem);
}
