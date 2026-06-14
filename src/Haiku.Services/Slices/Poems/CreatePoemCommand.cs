using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Command to create a new poem authored by the specified user.
/// </summary>
/// <param name="AuthorId">The identifier of the author creating the poem.</param>
/// <param name="Content">The full text of the poem, including optional <c>#tag</c> markers for auto-tagging.</param>
/// <param name="PoemType">Optional explicit poem type. When <c>null</c>, the type is auto-detected from content via <see cref="PoemEngine"/>.</param>
/// <param name="IsDraft">When <c>true</c>, the poem is saved as a draft and excluded from public listings.</param>
public record CreatePoemCommand(Guid AuthorId, string Content, PoemType? PoemType = null, bool IsDraft = false)
    : ICommand<Poem>;
