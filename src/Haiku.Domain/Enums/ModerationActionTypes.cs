namespace Haiku.Domain.Enums;

/// <summary>
/// Defines the set of moderation actions that can be applied to content or users.
/// </summary>
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
