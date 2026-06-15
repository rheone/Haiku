using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Dictionary.Domain;

/// <summary>
/// Represents a word in the custom pronunciation dictionary, supplementing the built-in CMU dictionary.
/// </summary>
/// <remarks>
/// <para>
/// Words in this table are user-contributed and must be approved (<see cref="IsApproved"/>)
/// before they are consumed by <c>SyllableEngine</c> for syllable counting. The built-in
/// CMU-based dictionary is loaded from <c>dictionary.dic</c> at startup.
/// </para>
/// </remarks>
[Table("CustomDictionaryWords")]
public class CustomDictionaryWord
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The word's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the word text, up to 200 characters.
    /// </summary>
    /// <value>The word text.</value>
    [Required]
    [MaxLength(200)]
    public string Word { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of syllables in this word as specified by the contributor.
    /// </summary>
    /// <value>The syllable count; must be greater than zero.</value>
    public int SyllableCount { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user who added this word to the dictionary.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the contributor.</value>
    [Required]
    public Guid AddedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user who added this word.
    /// </summary>
    /// <value>The <see cref="User"/> who contributed this word.</value>
    [ForeignKey(nameof(AddedByUserId))]
    public User AddedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the word was added.
    /// </summary>
    /// <value>The UTC addition timestamp.</value>
    public DateTime AddedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this word is approved for use in syllable counting.
    /// </summary>
    /// <value><c>true</c> if the word is approved and visible to the syllable engine; otherwise <c>false</c>.</value>
    public bool IsApproved { get; set; }
}
