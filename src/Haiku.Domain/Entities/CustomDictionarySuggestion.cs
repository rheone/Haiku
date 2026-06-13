using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a user-submitted suggestion to add a word to the custom dictionary.
/// </summary>
[Table("CustomDictionarySuggestions")]
public class CustomDictionarySuggestion
{
    /// <summary>
    /// Gets or sets the unique identifier for the suggestion.
    /// </summary>
    /// <value>The unique identifier for the suggestion.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the suggested word text.
    /// </summary>
    /// <value>The suggested word text.</value>
    [Required]
    [MaxLength(200)]
    public string Word { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the syllable count proposed by the suggester.
    /// </summary>
    /// <value>The proposed syllable count.</value>
    public int SuggestedSyllableCount { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who submitted the suggestion.
    /// </summary>
    /// <value>The unique identifier of the suggester.</value>
    [Required]
    public Guid SuggestedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the user who submitted the suggestion.
    /// </summary>
    /// <value>The <see cref="User"/> who submitted the suggestion.</value>
    [ForeignKey(nameof(SuggestedByUserId))]
    public User SuggestedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional justification provided by the suggester.
    /// </summary>
    /// <value>The justification text, or <c>null</c>.</value>
    [MaxLength(200)]
    public string? Justification { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the moderator who reviewed the suggestion, or <c>null</c> if pending.
    /// </summary>
    /// <value>The unique identifier of the reviewer, or <c>null</c>.</value>
    public Guid? ReviewedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the moderator who reviewed the suggestion.
    /// </summary>
    /// <value>The <see cref="User"/> who reviewed the suggestion, or <c>null</c>.</value>
    [ForeignKey(nameof(ReviewedByUserId))]
    public User? ReviewedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the suggestion was reviewed, or <c>null</c> if still pending.
    /// </summary>
    /// <value>The review timestamp, or <c>null</c>.</value>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Gets or sets the current review status of the suggestion.
    /// </summary>
    /// <value>A <see cref="DictionarySuggestionStatus"/> value indicating the review state.</value>
    [Column(TypeName = "varchar(20)")]
    public DictionarySuggestionStatus Status { get; set; }
}
