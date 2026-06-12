using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface ILoveRepository
{
    Task<Love?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId);
    Task SaveAsync(Love love);
    Task DeleteAsync(Love love);
    Task<int> GetLoveCountAsync(Guid haikuId);
}
