using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for custom dictionary words and suggestions using EF Core.
/// </summary>
public class DictionaryRepository : IDictionaryRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public DictionaryRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<List<CustomDictionaryWord>> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionaryWords.Where(w => w.IsApproved).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CustomDictionaryWord?> GetWordAsync(string word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionaryWords.FirstOrDefaultAsync(w => w.Word == word, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveWordAsync(CustomDictionaryWord word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(word);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.CustomDictionaryWords.Add(word);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteWordAsync(CustomDictionaryWord word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.CustomDictionaryWords.Remove(word);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<CustomDictionarySuggestion>> GetSuggestionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionarySuggestions.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CustomDictionarySuggestion?> GetSuggestionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionarySuggestions.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveSuggestionAsync(CustomDictionarySuggestion suggestion, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(suggestion);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.CustomDictionarySuggestions.Add(suggestion);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
