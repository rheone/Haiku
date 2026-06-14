namespace Haiku.Domain.Enums;

// Tracks each suggestion through a three-state lifecycle: Pending, then
// either Approved or Rejected by a moderator.

/// <summary>
/// Represents the review state of a user-submitted dictionary word suggestion.
/// </summary>
/// <remarks>
/// <para>Users submit new words to the custom dictionary through suggestions. Each
/// suggestion transitions from Pending to either Approved (word is added to the
/// dictionary) or Rejected (declined by a moderator). Approved suggestions become
/// available as <see cref="Haiku.Domain.Entities.CustomDictionaryWord"/> entries.</para>
/// </remarks>
public enum DictionarySuggestionStatus
{
    /// <summary>
    /// The suggestion has been submitted and awaits moderator review.
    /// </summary>
    Pending,

    /// <summary>
    /// The suggestion has been accepted and is available in the custom dictionary.
    /// </summary>
    Approved,

    /// <summary>
    /// The suggestion has been declined by a moderator.
    /// </summary>
    Rejected,
}
