using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Submits a new word suggestion to the dictionary for moderation review. Creates a pending suggestion entry.
/// </summary>
/// <param name="Word">The suggested word text.</param>
/// <param name="SyllableCount">The suggested syllable count.</param>
/// <param name="SuggestedByUserId">The identifier of the user submitting the suggestion.</param>
/// <param name="Justification">Optional justification explaining why the word should be added.</param>
public record SubmitSuggestionCommand(string Word, int SyllableCount, Guid SuggestedByUserId, string? Justification)
    : ICommand<bool>;
