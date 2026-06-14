using System.Diagnostics.CodeAnalysis;

namespace Haiku.Services.Rhyming;

public interface IRhymeProvider
{
    bool TryGetRhymeKey(string word, [NotNullWhen(true)] out string? key);
}
