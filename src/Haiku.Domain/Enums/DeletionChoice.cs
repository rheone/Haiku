namespace Haiku.Domain.Enums;

// Presented to users during account deletion to choose whether their content is
// preserved (anonymized) or permanently removed.

/// <summary>
/// Specifies how a user's content is handled when they delete their account.
/// </summary>
/// <remarks>
/// <para>This enum is presented during the account deletion confirmation flow. The
/// Anonymize option preserves the user's published content for the community (author
/// identity becomes anonymous), while Remove purges the content entirely from the
/// data store.</para>
/// </remarks>
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
