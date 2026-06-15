using MicroMediator;

namespace Haiku.Modules.Dictionary.Commands;

/// <summary>
/// Adds a word directly to the custom dictionary (pre-approved, bypassing the suggestion workflow).
/// Returns <c>false</c> if the word already exists.
/// </summary>
/// <param name="Word">The word text to add.</param>
/// <param name="SyllableCount">The number of syllables in the word.</param>
/// <param name="AddedByUserId">The identifier of the user adding the word.</param>
public record AddWordCommand(string Word, int SyllableCount, Guid AddedByUserId) : ICommand<bool>;
