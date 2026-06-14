using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haiku.Domain.Entities;

/// <summary>
/// Represents a poem bookmarked by a user for later reference.
/// </summary>
/// <remarks>
/// <para>
/// Bookmarks are a one-to-many relationship between users and poems, allowing users to
/// curate a personal reading list. Unlike <see cref="Love"/> or <see cref="Vote"/>, a bookmark
/// is a private organizational action that does not signal approval to other users.
/// </para>
/// </remarks>
public class Bookmark
{
    /// <summary>
    /// Gets or sets the primary key, a server-generated GUID.
    /// </summary>
    /// <value>The bookmark's unique database identifier.</value>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the user who saved the bookmark.
    /// </summary>
    /// <value>The <see cref="User"/> identifier who created the bookmark.</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the bookmarking user.
    /// </summary>
    /// <value>The <see cref="User"/> who created the bookmark.</value>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key to the bookmarked poem.
    /// </summary>
    /// <value>The <see cref="Poem"/> identifier that was bookmarked.</value>
    [Required]
    public Guid PoemId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the bookmarked poem.
    /// </summary>
    /// <value>The <see cref="Poem"/> that was bookmarked.</value>
    [ForeignKey(nameof(PoemId))]
    public Poem Poem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp (in UTC) when the bookmark was created.
    /// </summary>
    /// <value>The UTC creation timestamp.</value>
    public DateTime CreatedAt { get; set; }
}
