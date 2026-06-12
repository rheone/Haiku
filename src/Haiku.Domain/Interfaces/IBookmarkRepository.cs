using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IBookmarkRepository
{
    Task<Bookmark?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId);
    Task SaveAsync(Bookmark bookmark);
    Task DeleteAsync(Bookmark bookmark);
}
