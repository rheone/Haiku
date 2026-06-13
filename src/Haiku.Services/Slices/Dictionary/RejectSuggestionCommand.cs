using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Rejects a dictionary suggestion without creating a word entry.
/// Returns <c>false</c> if the suggestion is not found.
/// </summary>
/// <param name="SuggestionId">The identifier of the suggestion to reject.</param>
/// <param name="ReviewedByUserId">The identifier of the moderator reviewing the suggestion.</param>
public record RejectSuggestionCommand(Guid SuggestionId, Guid ReviewedByUserId) : ICommand<bool>;
