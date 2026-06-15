namespace Haiku.Modules.Dictionary.Application;

/// <summary>
/// Manages the custom syllable dictionary with a suggestion-and-review workflow.
/// </summary>
public class DictionaryService
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryService"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">Repository for custom dictionary entities.</param>
    public DictionaryService(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <summary>
    /// Looks up a word in the custom dictionary.
    /// </summary>
    /// <param name="word">The word to look up.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The dictionary entry if found; otherwise <c>null</c>.</returns>
    public async Task<CustomDictionaryWord?> GetWordAsync(string word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _dictionaryRepository.GetWordAsync(word, cancellationToken);
    }

    /// <summary>
    /// Retrieves all approved custom dictionary entries.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A list of all dictionary words.</returns>
    public async Task<List<CustomDictionaryWord>> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _dictionaryRepository.GetAllWordsAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a word to the custom dictionary. If the word already exists, this is a no-op.
    /// </summary>
    /// <param name="word">The word to add.</param>
    /// <param name="syllableCount">The known syllable count for this word.</param>
    /// <param name="addedByUserId">The ID of the moderator who added the entry.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddWordAsync(
        string word,
        int syllableCount,
        Guid addedByUserId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var existing = await _dictionaryRepository.GetWordAsync(word, cancellationToken);
        if (existing != null)
        {
            return;
        }

        var entry = new CustomDictionaryWord
        {
            Id = Guid.NewGuid(),
            Word = word,
            SyllableCount = syllableCount,
            AddedBy = new User { Id = addedByUserId },
            AddedAt = DateTime.UtcNow,
            IsApproved = true,
        };
        await _dictionaryRepository.SaveWordAsync(entry, cancellationToken);
    }

    /// <summary>
    /// Removes a word from the custom dictionary.
    /// </summary>
    /// <param name="word">The word to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the word was found and removed; <c>false</c> if it was not found.</returns>
    public async Task<bool> RemoveWordAsync(string word, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var existing = await _dictionaryRepository.GetWordAsync(word, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        await _dictionaryRepository.DeleteWordAsync(existing, cancellationToken);
        return true;
    }

    /// <summary>
    /// Submits a suggestion for a new dictionary entry.
    /// </summary>
    /// <remarks>
    /// <para>The suggestion is created in <see cref="DictionarySuggestionStatus.Pending"/> status and must be approved or rejected by a moderator.</para>
    /// </remarks>
    /// <param name="word">The suggested word.</param>
    /// <param name="syllableCount">The suggested syllable count.</param>
    /// <param name="suggestedByUserId">The ID of the user submitting the suggestion.</param>
    /// <param name="justification">Optional explanation for the suggestion.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SubmitSuggestionAsync(
        string word,
        int syllableCount,
        Guid suggestedByUserId,
        string? justification,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var suggestion = new CustomDictionarySuggestion
        {
            Id = Guid.NewGuid(),
            Word = word,
            SuggestedSyllableCount = syllableCount,
            SuggestedBy = new User { Id = suggestedByUserId },
            Justification = justification,
            Status = DictionarySuggestionStatus.Pending,
        };
        await _dictionaryRepository.SaveSuggestionAsync(suggestion, cancellationToken);
    }

    /// <summary>
    /// Approves a dictionary suggestion, promoting it to an approved dictionary entry.
    /// </summary>
    /// <param name="suggestionId">The ID of the suggestion to approve.</param>
    /// <param name="reviewedByUserId">The ID of the moderator approving the suggestion.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the suggestion was found and approved; <c>false</c> if not found.</returns>
    public async Task<bool> ApproveSuggestionAsync(
        Guid suggestionId,
        Guid reviewedByUserId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var suggestion = await _dictionaryRepository.GetSuggestionByIdAsync(suggestionId, cancellationToken);
        if (suggestion == null)
        {
            return false;
        }

        suggestion.Status = DictionarySuggestionStatus.Approved;
        suggestion.ReviewedBy = new User { Id = reviewedByUserId };
        suggestion.ReviewedAt = DateTime.UtcNow;
        await _dictionaryRepository.SaveSuggestionAsync(suggestion, cancellationToken);

        var entry = new CustomDictionaryWord
        {
            Id = Guid.NewGuid(),
            Word = suggestion.Word,
            SyllableCount = suggestion.SuggestedSyllableCount,
            AddedBy = new User { Id = reviewedByUserId },
            AddedAt = DateTime.UtcNow,
            IsApproved = true,
        };
        await _dictionaryRepository.SaveWordAsync(entry, cancellationToken);
        return true;
    }

    /// <summary>
    /// Rejects a dictionary suggestion without creating a dictionary entry.
    /// </summary>
    /// <param name="suggestionId">The ID of the suggestion to reject.</param>
    /// <param name="reviewedByUserId">The ID of the moderator rejecting the suggestion.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns><c>true</c> if the suggestion was found and rejected; <c>false</c> if not found.</returns>
    public async Task<bool> RejectSuggestionAsync(
        Guid suggestionId,
        Guid reviewedByUserId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var suggestion = await _dictionaryRepository.GetSuggestionByIdAsync(suggestionId, cancellationToken);
        if (suggestion == null)
        {
            return false;
        }

        suggestion.Status = DictionarySuggestionStatus.Rejected;
        suggestion.ReviewedBy = new User { Id = reviewedByUserId };
        suggestion.ReviewedAt = DateTime.UtcNow;
        await _dictionaryRepository.SaveSuggestionAsync(suggestion, cancellationToken);
        return true;
    }
}
