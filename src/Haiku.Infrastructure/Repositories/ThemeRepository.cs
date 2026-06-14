using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for <see cref="Theme"/> and <see cref="ThemeKeyword"/> entities using EF Core.
/// </summary>
public class ThemeRepository : IThemeRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public ThemeRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<Theme?> GetByIdAsync(Guid themeId, CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).FirstOrDefaultAsync(t => t.ThemeId == themeId, ct);
    }

    /// <inheritdoc/>
    public async Task<Theme?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).FirstOrDefaultAsync(t => t.Key == key, ct);
    }

    /// <inheritdoc/>
    public async Task<List<Theme>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).Where(t => t.Status == "Active").ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<Theme>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<ThemeKeyword>> GetKeywordsAsync(Guid themeId, CancellationToken ct = default)
    {
        return await _db.Set<ThemeKeyword>().Where(k => k.ThemeId == themeId).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Theme theme, CancellationToken ct = default)
    {
        if (theme.ThemeId == default)
        {
            _db.Set<Theme>().Add(theme);
        }
        else
        {
            _db.Set<Theme>().Update(theme);
        }
        await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid themeId, CancellationToken ct = default)
    {
        var theme = await _db.Set<Theme>().FindAsync(new object[] { themeId }, ct);
        if (theme != null)
        {
            _db.Set<Theme>().Remove(theme);
            await _db.SaveChangesAsync(ct);
        }
    }
}
