using MicroMediator;

namespace Haiku.Modules.Dictionary.Commands;

/// <summary>
/// Handles submission of a new dictionary suggestion. Creates a pending suggestion
/// that must be approved by a moderator before the word is added to the dictionary.
/// </summary>
public class SubmitSuggestionCommandHandler : ICommandHandler<SubmitSuggestionCommand, bool>
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitSuggestionCommandHandler"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">The dictionary repository for data access.</param>
    public SubmitSuggestionCommandHandler(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(SubmitSuggestionCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var suggestion = new CustomDictionarySuggestion
        {
            Id = Guid.NewGuid(),
            Word = request.Word,
            SuggestedSyllableCount = request.SyllableCount,
            // EF Core stub: sets the FK by attaching a reference without loading the full entity.
            SuggestedBy = new User { Id = request.SuggestedByUserId },
            Justification = request.Justification,
            Status = DictionarySuggestionStatus.Pending,
        };
        await _dictionaryRepository.SaveSuggestionAsync(suggestion, cancellationToken);
        return true;
    }
}
