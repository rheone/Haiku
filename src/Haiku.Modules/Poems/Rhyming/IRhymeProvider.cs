using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Rhyming;

/// <summary>
///     Provides rhyme key lookups for words. A rhyme key groups words that rhyme
///     together — two words rhyme if they produce the same key.
/// </summary>
public interface IRhymeProvider
{
    /// <summary>
    ///     Attempts to retrieve the rhyme key for <paramref name="word"/>.
    /// </summary>
    /// <param name="word">The word to look up.</param>
    /// <param name="key">When successful, the rhyme key; <c>null</c> otherwise.</param>
    /// <returns><c>true</c> if a rhyme key was found; <c>false</c> otherwise.</returns>
    bool TryGetRhymeKey(string word, [NotNullWhen(true)] out string? key);
}
