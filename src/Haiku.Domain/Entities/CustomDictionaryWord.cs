using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a custom word added to the pronunciation dictionary for syllable counting.
/// </summary>
[Table("CustomDictionaryWords")]
public class CustomDictionaryWord
{
    /// <summary>
    /// Gets or sets the unique identifier for the dictionary word.
    /// </summary>
    /// <value>The unique identifier for the dictionary word.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the word text.
    /// </summary>
    /// <value>The word text.</value>
    [Required]
    [MaxLength(200)]
    public string Word { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of syllables in the word as determined by the contributor.
    /// </summary>
    /// <value>The syllable count for this word.</value>
    public int SyllableCount { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who added the word.
    /// </summary>
    /// <value>The unique identifier of the adding user.</value>
    [Required]
    public Guid AddedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the user who added the word to the dictionary.
    /// </summary>
    /// <value>The <see cref="User"/> who added the word.</value>
    [ForeignKey(nameof(AddedByUserId))]
    public User AddedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the word was added.
    /// </summary>
    /// <value>The word addition timestamp.</value>
    public DateTime AddedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the word has been approved for use in syllable counting.
    /// </summary>
    /// <value><c>true</c> if the word is approved; otherwise <c>false</c>.</value>
    public bool IsApproved { get; set; }
}
