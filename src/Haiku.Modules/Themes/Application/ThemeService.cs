namespace Haiku.Modules.Themes.Application;

/// <summary>
/// Manages writing prompt themes, including retrieval, creation, update, and deletion.
/// </summary>
public class ThemeService
{
    private readonly IThemeRepository _themeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="themeRepository">Repository for theme entities.</param>
    public ThemeService(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    /// <summary>
    /// Gets all themes that are currently active and available for use in writing prompts.
    /// </summary>
    /// <returns>A list of active <see cref="Theme"/> entities.</returns>
    public async Task<List<Theme>> GetActiveThemesAsync()
    {
        return await _themeRepository.GetActiveAsync();
    }

    /// <summary>
    /// Gets a theme by its unique key identifier.
    /// </summary>
    /// <param name="key">The theme key (e.g. "seasonal", "nature").</param>
    /// <returns>The matching <see cref="Theme"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Theme?> GetThemeByKeyAsync(string key)
    {
        return await _themeRepository.GetByKeyAsync(key);
    }

    /// <summary>
    /// Gets a theme by its unique identifier.
    /// </summary>
    /// <param name="id">The theme identifier.</param>
    /// <returns>The matching <see cref="Theme"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Theme?> GetThemeByIdAsync(Guid id)
    {
        return await _themeRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Gets all themes regardless of active status.
    /// </summary>
    /// <returns>A list of all <see cref="Theme"/> entities.</returns>
    public async Task<List<Theme>> GetAllThemesAsync()
    {
        return await _themeRepository.GetAllAsync();
    }

    /// <summary>
    /// Creates a new theme, assigning a new identifier and setting timestamps.
    /// </summary>
    /// <param name="theme">The theme to create. Its <see cref="Theme.ThemeId"/> will be overwritten.</param>
    /// <returns>The created <see cref="Theme"/> with generated identifier and timestamps populated.</returns>
    public async Task<Theme> CreateThemeAsync(Theme theme)
    {
        theme.ThemeId = Guid.NewGuid();
        theme.CreatedAt = DateTime.UtcNow;
        theme.UpdatedAt = DateTime.UtcNow;
        await _themeRepository.SaveAsync(theme);
        return theme;
    }

    /// <summary>
    /// Updates an existing theme, refreshing its update timestamp.
    /// </summary>
    /// <param name="theme">The theme with modified properties. Requires a valid <see cref="Theme.ThemeId"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task UpdateThemeAsync(Theme theme)
    {
        theme.UpdatedAt = DateTime.UtcNow;
        await _themeRepository.SaveAsync(theme);
    }

    /// <summary>
    /// Deletes a theme by its identifier.
    /// </summary>
    /// <param name="themeId">The identifier of the theme to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task DeleteThemeAsync(Guid themeId)
    {
        await _themeRepository.DeleteAsync(themeId);
    }
}
