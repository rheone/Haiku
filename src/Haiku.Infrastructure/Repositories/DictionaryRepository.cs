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

    /// <summary>
    /// Retrieves all approved custom dictionary words.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of approved custom dictionary words.</returns>
    public async Task<List<CustomDictionaryWord>> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionaryWords.Where(w => w.IsApproved).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific custom dictionary word by its text.
    /// </summary>
    /// <param name="word">The word text to look up.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The custom dictionary word if found; otherwise <c>null</c>.</returns>
    public async Task<CustomDictionaryWord?> GetWordAsync(string word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionaryWords.FirstOrDefaultAsync(w => w.Word == word, cancellationToken);
    }

    /// <summary>
    /// Persists a new custom dictionary word or saves changes to an existing tracked word.
    /// </summary>
    /// <param name="word">The custom dictionary word entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveWordAsync(CustomDictionaryWord word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(word);
        if (entry.State == EntityState.Detached)
        {
            _db.CustomDictionaryWords.Add(word);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a custom dictionary word from the database.
    /// </summary>
    /// <param name="word">The custom dictionary word entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteWordAsync(CustomDictionaryWord word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.CustomDictionaryWords.Remove(word);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all pending and reviewed custom dictionary suggestions.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of all custom dictionary suggestions.</returns>
    public async Task<List<CustomDictionarySuggestion>> GetSuggestionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionarySuggestions.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a custom dictionary suggestion by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the suggestion.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The suggestion if found; otherwise <c>null</c>.</returns>
    public async Task<CustomDictionarySuggestion?> GetSuggestionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.CustomDictionarySuggestions.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Persists a new custom dictionary suggestion or saves changes to an existing tracked suggestion.
    /// </summary>
    /// <param name="suggestion">The custom dictionary suggestion entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveSuggestionAsync(CustomDictionarySuggestion suggestion, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(suggestion);
        if (entry.State == EntityState.Detached)
        {
            _db.CustomDictionarySuggestions.Add(suggestion);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
