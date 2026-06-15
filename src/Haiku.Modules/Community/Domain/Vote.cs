using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Modules.Community.Domain;

/// <summary>
/// Represents an upvote or downvote cast by a user on a poem.
/// </summary>
/// <remarks>
/// <para>
/// Votes are directional: <see cref="Value"/> is positive (+1) for upvotes and negative (-1)
/// for downvotes. A user may vote on a given poem only once; subsequent votes replace the
/// previous value rather than creating duplicates. Unlike <see cref="Love"/>, which is an
/// exclusively positive signal, votes allow for nuanced community feedback.
/// </para>
/// </remarks>
public class Vote
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The vote record's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the poem being voted on.
    /// </summary>
    /// <value>The <see cref="Poem"/> identifier of the poem being voted on.</value>
    [Required]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the poem being voted on.
    /// </summary>
    /// <value>The <see cref="Poem"/> being voted on.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key to the user who cast the vote.
    /// </summary>
    /// <value>The <see cref="User"/> identifier of the voting user.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the voting user.
    /// </summary>
    /// <value>The <see cref="User"/> who cast the vote.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the vote value indicating direction.
    /// </summary>
    /// <value>+1 for upvote, -1 for downvote. Other values are not used but are not validated at the entity level.</value>
    public sbyte Value { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the vote was cast.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
