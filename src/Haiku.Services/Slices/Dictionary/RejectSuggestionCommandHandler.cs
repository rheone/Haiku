using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Handles rejection of a dictionary suggestion. Marks the suggestion as rejected
/// without creating a dictionary entry.
/// </summary>
public class RejectSuggestionCommandHandler : ICommandHandler<RejectSuggestionCommand, bool>
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RejectSuggestionCommandHandler"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">The dictionary repository for data access.</param>
    public RejectSuggestionCommandHandler(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(RejectSuggestionCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var suggestion = await _dictionaryRepository.GetSuggestionByIdAsync(request.SuggestionId, cancellationToken);
        if (suggestion == null)
        {
            return false;
        }

        suggestion.Status = DictionarySuggestionStatus.Rejected;
        suggestion.ReviewedBy = new User { Id = request.ReviewedByUserId };
        suggestion.ReviewedAt = DateTime.UtcNow;
        await _dictionaryRepository.SaveSuggestionAsync(suggestion, cancellationToken);
        return true;
    }
}
