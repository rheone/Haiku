using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a user-submitted suggestion to add a word to the custom pronunciation dictionary.
/// </summary>
/// <remarks>
/// <para>
/// Moderators review suggestions and may accept or reject them. Accepted suggestions
/// become <see cref="CustomDictionaryWord"/> entries used by the syllable-counting engine.
/// </para>
/// </remarks>
[Table("CustomDictionarySuggestions")]
public class CustomDictionarySuggestion
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The suggestion's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the suggested word text, up to 200 characters.
    /// </summary>
    /// <value>The suggested word text.</value>
    [Required]
    [MaxLength(200)]
    public string Word { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the syllable count proposed by the suggester.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This count is a suggestion only; the reviewing moderator may override it when approving the word.
    /// </para>
    /// </remarks>
    /// <value>The proposed syllable count, must be greater than zero.</value>
    public int SuggestedSyllableCount { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user who submitted the suggestion.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the suggester.</value>
    [Required]
    public Guid SuggestedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user who submitted the suggestion.
    /// </summary>
    /// <value>The <see cref="User"/> who submitted the suggestion.</value>
    [ForeignKey(nameof(SuggestedByUserId))]
    public User SuggestedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional justification text explaining why this word should be added.
    /// </summary>
    /// <value>The justification text, up to 200 characters, or <c>null</c> if omitted.</value>
    [MaxLength(200)]
    public string? Justification { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the moderator who reviewed the suggestion.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the reviewing moderator, or <c>null</c> while pending.</value>
    public Guid? ReviewedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the reviewing moderator.
    /// </summary>
    /// <value>The <see cref="User"/> who reviewed the suggestion, or <c>null</c> while pending.</value>
    [ForeignKey(nameof(ReviewedByUserId))]
    public User? ReviewedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the suggestion was reviewed.
    /// </summary>
    /// <value>The UTC review timestamp, or <c>null</c> while pending.</value>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Gets or sets the current review status of this suggestion.
    /// </summary>
    /// <value>A <see cref="DictionarySuggestionStatus"/> value: <c>Pending</c>, <c>Approved</c>, or <c>Rejected</c>.</value>
    [Column(TypeName = "varchar(20)")]
    public DictionarySuggestionStatus Status { get; set; }
}
