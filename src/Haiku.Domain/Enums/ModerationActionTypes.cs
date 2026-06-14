namespace Haiku.Domain.Enums;

// String constants (not an enum) so values can be stored directly in the database
// as natural-language action labels.

/// <summary>
/// Defines the set of moderation actions that can be applied to content or users.
/// </summary>
/// <remarks>
/// <para>Each constant value pairs with a corresponding reverse action (Hide/Unhide,
/// Disable/Reinstate). The <see cref="TargetTypes"/> class identifies which entity
/// the action applies to. This pattern uses string constants rather than an enum so
/// the values are human-readable when stored in the moderation audit log.</para>
/// </remarks>
public static class ModerationActionTypes
{
    /// <summary>
    /// Marks content as hidden from public view while preserving the data.
    /// </summary>
    public const string Hide = "Hide";

    /// <summary>
    /// Restores previously hidden content to public visibility.
    /// </summary>
    public const string Unhide = "Unhide";

    /// <summary>
    /// Disables a user account, preventing further interaction with the platform.
    /// </summary>
    public const string Disable = "Disable";

    /// <summary>
    /// Reinstates a previously disabled user account.
    /// </summary>
    public const string Reinstate = "Reinstate";
}
