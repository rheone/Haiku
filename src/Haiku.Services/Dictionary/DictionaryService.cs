using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;

namespace Haiku.Services.Dictionary;

public class DictionaryService
{
    private readonly IDictionaryRepository _dictionaryRepository;

    public DictionaryService(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    public async Task<CustomDictionaryWord?> GetWordAsync(string word) =>
        await _dictionaryRepository.GetWordAsync(word);

    public async Task<List<CustomDictionaryWord>> GetAllWordsAsync() =>
        await _dictionaryRepository.GetAllWordsAsync();

    public async Task AddWordAsync(string word, int syllableCount, Guid addedByUserId)
    {
        var existing = await _dictionaryRepository.GetWordAsync(word);
        if (existing != null) return;

        var entry = new CustomDictionaryWord
        {
            Id = Guid.NewGuid(),
            Word = word,
            SyllableCount = syllableCount,
            AddedBy = new User { Id = addedByUserId },
            AddedAt = DateTime.UtcNow,
            IsApproved = true
        };
        await _dictionaryRepository.SaveWordAsync(entry);
    }

    public async Task<bool> RemoveWordAsync(Guid wordId)
    {
        var word = await _dictionaryRepository.GetWordAsync(wordId.ToString());
        if (word == null) return false;

        await _dictionaryRepository.DeleteWordAsync(word);
        return true;
    }

    public async Task SubmitSuggestionAsync(string word, int syllableCount, Guid suggestedByUserId, string? justification)
    {
        var suggestion = new CustomDictionarySuggestion
        {
            Id = Guid.NewGuid(),
            Word = word,
            SuggestedSyllableCount = syllableCount,
            SuggestedBy = new User { Id = suggestedByUserId },
            Justification = justification,
            Status = DictionarySuggestionStatus.Pending
        };
        await _dictionaryRepository.SaveSuggestionAsync(suggestion);
    }

    public async Task<bool> ApproveSuggestionAsync(Guid suggestionId, Guid reviewedByUserId)
    {
        var suggestion = await _dictionaryRepository.GetSuggestionByIdAsync(suggestionId);
        if (suggestion == null) return false;

        suggestion.Status = DictionarySuggestionStatus.Approved;
        suggestion.ReviewedBy = new User { Id = reviewedByUserId };
        suggestion.ReviewedAt = DateTime.UtcNow;
        await _dictionaryRepository.SaveSuggestionAsync(suggestion);

        var entry = new CustomDictionaryWord
        {
            Id = Guid.NewGuid(),
            Word = suggestion.Word,
            SyllableCount = suggestion.SuggestedSyllableCount,
            AddedBy = new User { Id = reviewedByUserId },
            AddedAt = DateTime.UtcNow,
            IsApproved = true
        };
        await _dictionaryRepository.SaveWordAsync(entry);
        return true;
    }

    public async Task<bool> RejectSuggestionAsync(Guid suggestionId, Guid reviewedByUserId)
    {
        var suggestion = await _dictionaryRepository.GetSuggestionByIdAsync(suggestionId);
        if (suggestion == null) return false;

        suggestion.Status = DictionarySuggestionStatus.Rejected;
        suggestion.ReviewedBy = new User { Id = reviewedByUserId };
        suggestion.ReviewedAt = DateTime.UtcNow;
        await _dictionaryRepository.SaveSuggestionAsync(suggestion);
        return true;
    }
}
