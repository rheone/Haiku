using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class DictionaryRepository : IDictionaryRepository
{
    private readonly ISession _session;

    public DictionaryRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<List<CustomDictionaryWord>> GetAllWordsAsync() =>
        await _session.Query<CustomDictionaryWord>().Where(w => w.IsApproved).ToListAsync();

    public async Task<CustomDictionaryWord?> GetWordAsync(string word) =>
        await _session.Query<CustomDictionaryWord>()
            .FirstOrDefaultAsync(w => w.Word == word);

    public async Task SaveWordAsync(CustomDictionaryWord word)
    {
        await _session.SaveOrUpdateAsync(word);
        await _session.FlushAsync();
    }

    public async Task DeleteWordAsync(CustomDictionaryWord word)
    {
        await _session.DeleteAsync(word);
        await _session.FlushAsync();
    }

    public async Task<List<CustomDictionarySuggestion>> GetSuggestionsAsync() =>
        await _session.Query<CustomDictionarySuggestion>().ToListAsync();

    public async Task<CustomDictionarySuggestion?> GetSuggestionByIdAsync(Guid id) =>
        await _session.GetAsync<CustomDictionarySuggestion>(id);

    public async Task SaveSuggestionAsync(CustomDictionarySuggestion suggestion)
    {
        await _session.SaveOrUpdateAsync(suggestion);
        await _session.FlushAsync();
    }
}
