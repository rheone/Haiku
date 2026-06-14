namespace Haiku.Services.Syllables;

public interface IWordTokenizer
{
    TokenizedLine Tokenize(string line);
}
