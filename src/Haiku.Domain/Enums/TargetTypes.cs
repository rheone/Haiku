namespace Haiku.Domain.Enums;

/// <summary>
/// Defines the target type identifiers used as string discriminators for moderation targets.
/// </summary>
public static class TargetTypes
{
    /// <summary>
    /// Identifies a poem as the target of a moderation action.
    /// </summary>
    public const string Poem = "Poem";

    /// <summary>
    /// Identifies a user account as the target of a moderation action.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Identifies a custom dictionary word as the target of a moderation action.
    /// </summary>
    public const string DictionaryWord = "DictionaryWord";

    /// <summary>
    /// Identifies a dictionary suggestion as the target of a moderation action.
    /// </summary>
    public const string DictionarySuggestion = "DictionarySuggestion";
}
