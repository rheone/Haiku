using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IThemeRepository
{
    Task<Theme?> GetByIdAsync(Guid themeId, CancellationToken ct = default);
    Task<Theme?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<List<Theme>> GetActiveAsync(CancellationToken ct = default);
    Task<List<Theme>> GetAllAsync(CancellationToken ct = default);
    Task<List<ThemeKeyword>> GetKeywordsAsync(Guid themeId, CancellationToken ct = default);
    Task SaveAsync(Theme theme, CancellationToken ct = default);
    Task DeleteAsync(Guid themeId, CancellationToken ct = default);
}
