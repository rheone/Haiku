using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Approves a dictionary suggestion and creates the corresponding custom dictionary word.
/// Returns <c>false</c> if the suggestion is not found.
/// </summary>
/// <param name="SuggestionId">The identifier of the suggestion to approve.</param>
/// <param name="ReviewedByUserId">The identifier of the moderator reviewing the suggestion.</param>
public record ApproveSuggestionCommand(Guid SuggestionId, Guid ReviewedByUserId) : ICommand<bool>;
