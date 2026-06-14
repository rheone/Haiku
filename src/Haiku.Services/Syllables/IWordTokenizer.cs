namespace Haiku.Services.Syllables;

/// <summary>
///     Splits a line of poem text into word tokens for syllable counting.
/// </summary>
public interface IWordTokenizer
{
    /// <summary>
    ///     Tokenizes a line into its constituent word tokens, removing non-spoken characters.
    /// </summary>
    /// <param name="line">The line of text to tokenize.</param>
    /// <returns>A <see cref="TokenizedLine"/> with the extracted words and metadata.</returns>
    TokenizedLine Tokenize(string line);
}
