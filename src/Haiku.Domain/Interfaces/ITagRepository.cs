using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByNameAsync(string name);
    Task<Tag> GetOrCreateAsync(string name);
}
