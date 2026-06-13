using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Handles approval of a dictionary suggestion. Marks the suggestion as approved and creates
/// the corresponding approved word entry in the custom dictionary in a single operation.
/// </summary>
public class ApproveSuggestionCommandHandler : ICommandHandler<ApproveSuggestionCommand, bool>
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApproveSuggestionCommandHandler"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">The dictionary repository for data access.</param>
    public ApproveSuggestionCommandHandler(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(ApproveSuggestionCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var suggestion = await _dictionaryRepository.GetSuggestionByIdAsync(request.SuggestionId, cancellationToken);
        if (suggestion == null)
        {
            return false;
        }

        suggestion.Status = DictionarySuggestionStatus.Approved;
        suggestion.ReviewedBy = new User { Id = request.ReviewedByUserId };
        suggestion.ReviewedAt = DateTime.UtcNow;
        await _dictionaryRepository.SaveSuggestionAsync(suggestion, cancellationToken);

        var entry = new CustomDictionaryWord
        {
            Id = Guid.NewGuid(),
            Word = suggestion.Word,
            SyllableCount = suggestion.SuggestedSyllableCount,
            AddedBy = new User { Id = request.ReviewedByUserId },
            AddedAt = DateTime.UtcNow,
            IsApproved = true,
        };
        await _dictionaryRepository.SaveWordAsync(entry, cancellationToken);
        return true;
    }
}
