namespace Haiku.Domain.Enums;

/// <summary>
/// Represents the review state of a user-submitted dictionary word suggestion.
/// </summary>
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
