using System.Text.RegularExpressions;

namespace Haiku.Modules.Shared.Domain.ValueObjects;

/// <summary>
/// Extension methods for <see cref="PoemType"/> enum values.
/// </summary>
public static class PoemTypeExtensions
{
    /// <summary>
    /// Converts a PascalCase <see cref="PoemType"/> name to the kebab-case TypeId convention.
    /// Examples: <c>SyllableCrestWave</c> → <c>"syllable-crest-wave"</c>, <c>Haiku</c> → <c>"haiku"</c>.
    /// </summary>
    /// <param name="poemType">The <see cref="PoemType"/> enum value to convert.</param>
    /// <returns>The kebab-case representation of the enum value (e.g., "syllable-crest-wave").</returns>
    public static string ToKebabCase(this PoemType poemType) =>
        Regex.Replace(poemType.ToString(), "(?<=[a-z])([A-Z])", "-$1").ToLowerInvariant();
}
