using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables;

public interface ISyllableProvider
{
    bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result);
}
