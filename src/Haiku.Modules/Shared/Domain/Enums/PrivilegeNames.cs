namespace Haiku.Modules.Shared.Domain.Enums;

// Privilege identifiers are stored as strings in the database and checked
// at runtime via IModerationRepository.HasPrivilegeAsync. The snake_case
// convention matches how they are stored and compared.

/// <summary>
/// Defines the privilege identifiers used for role-based authorization.
/// </summary>
/// <remarks>
/// <para>Privileges are granular permissions assigned to users, stored as string
/// values in the database. Unlike simple roles, privileges can be granted and
/// revoked individually. Policy-based authorization in ASP.NET Core references
/// these identifiers at the application layer.</para>
/// </remarks>
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
