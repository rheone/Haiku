using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;

namespace Haiku.Services;

public class ThemeService
{
    private readonly IThemeRepository _themeRepository;

    public ThemeService(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    public async Task<List<Theme>> GetActiveThemesAsync()
    {
        return await _themeRepository.GetActiveAsync();
    }

    public async Task<Theme?> GetThemeByKeyAsync(string key)
    {
        return await _themeRepository.GetByKeyAsync(key);
    }

    public async Task<Theme?> GetThemeByIdAsync(Guid id)
    {
        return await _themeRepository.GetByIdAsync(id);
    }

    public async Task<List<Theme>> GetAllThemesAsync()
    {
        return await _themeRepository.GetAllAsync();
    }

    public async Task<Theme> CreateThemeAsync(Theme theme)
    {
        theme.ThemeId = Guid.NewGuid();
        theme.CreatedAt = DateTime.UtcNow;
        theme.UpdatedAt = DateTime.UtcNow;
        await _themeRepository.SaveAsync(theme);
        return theme;
    }

    public async Task UpdateThemeAsync(Theme theme)
    {
        theme.UpdatedAt = DateTime.UtcNow;
        await _themeRepository.SaveAsync(theme);
    }

    public async Task DeleteThemeAsync(Guid themeId)
    {
        await _themeRepository.DeleteAsync(themeId);
    }
}
