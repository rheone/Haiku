using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Removes a word from the custom dictionary. Returns <c>false</c> if the word is not found.
/// </summary>
/// <param name="Word">The word text to remove.</param>
public record RemoveWordCommand(string Word) : ICommand<bool>;
