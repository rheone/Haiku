using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IVoteRepository
{
    Task<Vote?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId);
    Task SaveAsync(Vote vote);
    Task DeleteAsync(Vote vote);
    Task<int> GetNetScoreAsync(Guid haikuId);
}
