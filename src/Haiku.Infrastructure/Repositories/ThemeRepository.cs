using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

public class ThemeRepository : IThemeRepository
{
    private readonly HaikuDbContext _db;

    public ThemeRepository(HaikuDbContext db)
    {
        _db = db;
    }

    public async Task<Theme?> GetByIdAsync(Guid themeId, CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).FirstOrDefaultAsync(t => t.ThemeId == themeId, ct);
    }

    public async Task<Theme?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).FirstOrDefaultAsync(t => t.Key == key, ct);
    }

    public async Task<List<Theme>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).Where(t => t.Status == "Active").ToListAsync(ct);
    }

    public async Task<List<Theme>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Set<Theme>().Include(t => t.Keywords).ToListAsync(ct);
    }

    public async Task<List<ThemeKeyword>> GetKeywordsAsync(Guid themeId, CancellationToken ct = default)
    {
        return await _db.Set<ThemeKeyword>().Where(k => k.ThemeId == themeId).ToListAsync(ct);
    }

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
