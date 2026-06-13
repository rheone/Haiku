namespace Haiku.Domain.Enums;

/// <summary>
/// Specifies how a user's content is handled when they delete their account.
/// </summary>
public enum DeletionChoice
{
    /// <summary>
    /// The content remains on the platform with the author's identity replaced by an anonymous marker.
    /// </summary>
    Anonymize,

    /// <summary>
    /// The content is permanently deleted from the platform.
    /// </summary>
    Remove,
}
