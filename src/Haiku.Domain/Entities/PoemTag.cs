using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents the many-to-many link between a poem and a tag.
/// </summary>
[Table("PoemTags")]
public class PoemTag
{
    /// <summary>
    /// Gets or sets the unique identifier of the tagged poem.
    /// </summary>
    /// <value>The unique identifier of the poem.</value>
    [Key]
    [Column(Order = 0)]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the applied tag.
    /// </summary>
    /// <value>The unique identifier of the tag.</value>
    [Key]
    [Column(Order = 1)]
    public int TagId { get; set; }

    /// <summary>
    /// Gets or sets the tagged poem.
    /// </summary>
    /// <value>The <see cref="Entities.Poem"/> associated with this tag link.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the tag applied to the poem.
    /// </summary>
    /// <value>The <see cref="Entities.Tag"/> applied to the poem.</value>
    [ForeignKey(nameof(TagId))]
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Determines whether the specified object is equal to the current poem-tag link.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified object is a <see cref="PoemTag"/> with the same poem and tag identifiers; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is PoemTag other && PoemId == other.PoemId && TagId == other.TagId;

    /// <summary>
    /// Returns a hash code for this poem-tag link based on the composite poem and tag identifiers.
    /// </summary>
    /// <returns>A hash code computed from <see cref="PoemId"/> and <see cref="TagId"/>.</returns>
    public override int GetHashCode() => HashCode.Combine(PoemId, TagId);
}
