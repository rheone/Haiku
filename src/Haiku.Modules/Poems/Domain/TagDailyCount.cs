using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Poems.Domain;

/// <summary>
/// Tracks the aggregate number of times a tag was applied to poems on a specific calendar date.
/// </summary>
/// <remarks>
/// <para>
/// This is a read-optimized denormalized counter used for tag cloud displays and
/// trending tag analysis. The count is updated incrementally when poems are tagged
/// or tags are removed. The composite primary key is (TagId, Date).
/// </para>
/// </remarks>
[Table("TagDailyCounts")]
public class TagDailyCount
{
    /// <summary>
    /// Gets or sets the foreign key to the tag being counted (part of the composite primary key).
    /// </summary>
    /// <value>The <see cref="Tag"/> identifier.</value>
    [Key]
    [Column(Order = 0)]
    public int TagId { get; set; }

    /// <summary>
    /// Gets or sets the calendar date for which the count applies (part of the composite primary key).
    /// </summary>
    /// <value>The calendar date (date-only, no time component).</value>
    [Key]
    [Column(Order = 1)]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the number of tagged poems for this tag on this date.
    /// </summary>
    /// <value>The usage count. A value of zero indicates no active usage on this date.</value>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated tag.
    /// </summary>
    /// <value>The <see cref="Tag"/> for this daily count record.</value>
    [ForeignKey(nameof(TagId))]
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Determines whether the specified object equals this daily count record.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Overridden because <see cref="TagDailyCount"/> uses a composite key and is
    /// used in collection operations where value equality is expected.
    /// </para>
    /// </remarks>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="TagDailyCount"/> with matching <see cref="TagId"/> and <see cref="Date"/>; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is TagDailyCount other && TagId == other.TagId && Date == other.Date;

    /// <summary>
    /// Returns a hash code for this daily count record based on its composite key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <c>HashCode.Combine(TagId, Date)</c> to produce a stable hash
    /// consistent with <see cref="Equals(object?)"/>.
    /// </para>
    /// </remarks>
    /// <returns>A hash code computed from <see cref="TagId"/> and <see cref="Date"/>.</returns>
    public override int GetHashCode() => HashCode.Combine(TagId, Date);
}
