using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides data access for theme and theme keyword entities.
/// </summary>
/// <remarks>
/// <para>Themes are curated writing prompts with configurable visual palettes and keyword-based
/// matching rules. Each theme has a unique string key for code and URL references. This
/// repository manages both the themes themselves and their keyword associations used for
/// automatic content-based theme recommendation.</para>
/// </remarks>
public interface IThemeRepository
{
    /// <summary>Retrieves a theme by its unique identifier.</summary>
    /// <param name="themeId">The unique identifier of the theme.</param>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching theme, or <c>null</c> if not found.</returns>
    Task<Theme?> GetByIdAsync(Guid themeId, CancellationToken ct = default);

    /// <summary>Retrieves a theme by its unique string key.</summary>
    /// <param name="key">The unique string key identifying the theme (e.g., "ocean", "sunset").</param>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching theme, or <c>null</c> if not found.</returns>
    Task<Theme?> GetByKeyAsync(string key, CancellationToken ct = default);

    /// <summary>Retrieves all currently active (published) themes.</summary>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of active theme entities.</returns>
    Task<List<Theme>> GetActiveAsync(CancellationToken ct = default);

    /// <summary>Retrieves all themes regardless of publication status.</summary>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of all theme entities.</returns>
    Task<List<Theme>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Retrieves all keyword-weight pairs associated with a theme.</summary>
    /// <param name="themeId">The unique identifier of the theme.</param>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of keyword entities for the specified theme.</returns>
    Task<List<ThemeKeyword>> GetKeywordsAsync(Guid themeId, CancellationToken ct = default);

    /// <summary>Persists a new or modified theme.</summary>
    /// <param name="theme">The theme entity to save.</param>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(Theme theme, CancellationToken ct = default);

    /// <summary>Removes a theme by its unique identifier.</summary>
    /// <param name="themeId">The unique identifier of the theme to delete.</param>
    /// <param name="ct">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid themeId, CancellationToken ct = default);
}
