using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

// Manages both the custom dictionary (approved words) and user-submitted suggestions.
// The custom dictionary supplements the CMU-based syllable engine for domain-specific
// words not in the standard pronunciation dictionary.

/// <summary>
/// Provides data access for the custom dictionary and user-submitted word suggestions.
/// </summary>
/// <remarks>
/// <para>The syllable engine loads all words from the custom dictionary at startup,
/// supplementing the built-in CMU pronunciation database. Users can suggest new words,
/// which moderators approve or reject. Once approved, a word becomes available for
/// syllable counting across all poems.</para>
/// </remarks>
public interface IDictionaryRepository
{
    /// <summary>
    /// Retrieves all words in the custom dictionary.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of all custom dictionary words.</returns>
    Task<List<CustomDictionaryWord>> GetAllWordsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Looks up a custom dictionary word by its exact text.
    /// </summary>
    /// <param name="word">The exact word text to search for.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching dictionary word, or <c>null</c> if not found.</returns>
    Task<CustomDictionaryWord?> GetWordAsync(string word, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new or modified custom dictionary word.
    /// </summary>
    /// <param name="word">The dictionary word entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveWordAsync(CustomDictionaryWord word, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a word from the custom dictionary.
    /// </summary>
    /// <param name="word">The dictionary word entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteWordAsync(CustomDictionaryWord word, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all user-submitted dictionary suggestions.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of all dictionary suggestions.</returns>
    Task<List<CustomDictionarySuggestion>> GetSuggestionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a dictionary suggestion by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the suggestion.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The matching suggestion, or <c>null</c> if not found.</returns>
    Task<CustomDictionarySuggestion?> GetSuggestionByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new or modified dictionary suggestion.
    /// </summary>
    /// <param name="suggestion">The suggestion entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveSuggestionAsync(CustomDictionarySuggestion suggestion, CancellationToken cancellationToken = default);
}
