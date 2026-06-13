using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Tracks the number of times a tag was used on a given calendar date.
/// </summary>
[Table("TagDailyCounts")]
public class TagDailyCount
{
    /// <summary>
    /// Gets or sets the unique identifier of the tag.
    /// </summary>
    /// <value>The unique identifier of the tag.</value>
    [Key]
    [Column(Order = 0)]
    public int TagId { get; set; }

    /// <summary>
    /// Gets or sets the calendar date for which usage is counted.
    /// </summary>
    /// <value>The calendar date.</value>
    [Key]
    [Column(Order = 1)]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the number of poems tagged with this tag on this date.
    /// </summary>
    /// <value>The usage count for this tag on this date.</value>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the tag associated with this daily count.
    /// </summary>
    /// <value>The <see cref="Entities.Tag"/> for this daily count.</value>
    [ForeignKey(nameof(TagId))]
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Determines whether the specified object is equal to the current daily count record.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified object is a <see cref="TagDailyCount"/> with the same tag and date; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is TagDailyCount other && TagId == other.TagId && Date == other.Date;

    /// <summary>
    /// Returns a hash code for this daily count record based on the composite tag and date identifiers.
    /// </summary>
    /// <returns>A hash code computed from <see cref="TagId"/> and <see cref="Date"/>.</returns>
    public override int GetHashCode() => HashCode.Combine(TagId, Date);
}
