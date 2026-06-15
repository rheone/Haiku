using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Poems.Domain;

/// <summary>
/// Represents the many-to-many join link between a poem and a tag.
/// </summary>
/// <remarks>
/// <para>
/// This is a composite-key join entity with <see cref="PoemId"/> and <see cref="TagId"/>
/// forming a composite primary key. Equality and hash code are based on these two keys,
/// enabling correct dictionary and set semantics for tag association lookups.
/// </para>
/// </remarks>
[Table("PoemTags")]
public class PoemTag
{
    /// <summary>
    /// Gets or sets the foreign key to the tagged poem (part of the composite primary key).
    /// </summary>
    /// <value>The <see cref="Poem"/> identifier.</value>
    [Key]
    [Column(Order = 0)]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the applied tag (part of the composite primary key).
    /// </summary>
    /// <value>The <see cref="Tag"/> identifier.</value>
    [Key]
    [Column(Order = 1)]
    public int TagId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the tagged poem.
    /// </summary>
    /// <value>The <see cref="Poem"/> associated with this tag link.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property to the applied tag.
    /// </summary>
    /// <value>The <see cref="Tag"/> applied to the poem.</value>
    [ForeignKey(nameof(TagId))]
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Determines whether the specified object equals this poem-tag link.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Overridden because <see cref="PoemTag"/> is a composite-key entity used in
    /// collection operations where value equality is required. Two links are equal
    /// if they reference the same poem and the same tag.
    /// </para>
    /// </remarks>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="PoemTag"/> with matching <see cref="PoemId"/> and <see cref="TagId"/>; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is PoemTag other && PoemId == other.PoemId && TagId == other.TagId;

    /// <summary>
    /// Returns a hash code for this poem-tag link based on its composite key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <c>HashCode.Combine(PoemId, TagId)</c> to produce a stable
    /// hash consistent with <see cref="Equals(object?)"/>.
    /// </para>
    /// </remarks>
    /// <returns>A hash code computed from <see cref="PoemId"/> and <see cref="TagId"/>.</returns>
    public override int GetHashCode() => HashCode.Combine(PoemId, TagId);
}
