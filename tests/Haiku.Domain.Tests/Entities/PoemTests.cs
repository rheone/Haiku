namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.Poem"/> entity instantiation and property assignment.
/// Poems are the central content entity, supporting soft-delete via <see cref="Haiku.Domain.Entities.Poem.DeletedAt"/>,
/// moderation hide via <see cref="Haiku.Domain.Entities.Poem.IsHidden"/>, and draft visibility
/// scoped to the author. The detected poetic form and total syllable count are set at creation time.
/// </summary>
public class PoemTests { }
