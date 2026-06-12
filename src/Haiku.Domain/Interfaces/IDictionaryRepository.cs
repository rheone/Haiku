using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IDictionaryRepository
{
    Task<List<CustomDictionaryWord>> GetAllWordsAsync();
    Task<CustomDictionaryWord?> GetWordAsync(string word);
    Task SaveWordAsync(CustomDictionaryWord word);
    Task DeleteWordAsync(CustomDictionaryWord word);
    Task<List<CustomDictionarySuggestion>> GetSuggestionsAsync();
    Task<CustomDictionarySuggestion?> GetSuggestionByIdAsync(Guid id);
    Task SaveSuggestionAsync(CustomDictionarySuggestion suggestion);
}
