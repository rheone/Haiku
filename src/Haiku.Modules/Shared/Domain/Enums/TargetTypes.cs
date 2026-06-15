namespace Haiku.Modules.Shared.Domain.Enums;

// Used as string discriminators in the ModerationAction entity to identify
// which entity type a moderation action targets.

/// <summary>
/// Defines the target type identifiers used as string discriminators for moderation targets.
/// </summary>
/// <remarks>
/// <para>Each <see cref="Haiku.Modules.Moderation.Domain.ModerationAction"/> records both an action (from
/// <see cref="ModerationActionTypes"/>) and a target type (from this class). The
/// target type tells the application which entity table to act upon when processing
/// the moderation action.</para>
/// </remarks>
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
