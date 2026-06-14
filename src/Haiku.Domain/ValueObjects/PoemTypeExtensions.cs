using System.Text.RegularExpressions;
using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

/// <summary>
/// Extension methods for <see cref="PoemType"/> enum values.
/// </summary>
public static class PoemTypeExtensions
{
    /// <summary>
    /// Converts a PascalCase <see cref="PoemType"/> name to the kebab-case TypeId convention.
    /// Examples: <c>SyllableCrestWave</c> → <c>"syllable-crest-wave"</c>, <c>Haiku</c> → <c>"haiku"</c>.
    /// </summary>
    public static string ToKebabCase(this PoemType poemType) =>
        Regex.Replace(poemType.ToString(), "(?<=[a-z])([A-Z])", "-$1").ToLowerInvariant();
}
