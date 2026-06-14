namespace Haiku.Domain.Enums;

/// <summary>
/// Defines the privilege identifiers used for role-based authorization.
/// </summary>
public static class PrivilegeNames
{
    /// <summary>
    /// Allows a user to moderate poems and poem content.
    /// </summary>
    public const string ModeratePoems = "moderate_poems";

    /// <summary>
    /// Allows a user to moderate other user accounts.
    /// </summary>
    public const string ModerateUsers = "moderate_users";

    /// <summary>
    /// Allows a user to add, edit, or remove words from the custom dictionary.
    /// </summary>
    public const string ManageDictionary = "manage_dictionary";

    /// <summary>
    /// Allows a user to view application log entries.
    /// </summary>
    public const string ViewLogs = "view_logs";
}
