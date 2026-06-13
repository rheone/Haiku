using Haiku.Domain.Entities;
using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Query to retrieve a poem by its identifier. Returns <c>null</c> when no matching poem exists.
/// </summary>
/// <param name="PoemId">The identifier of the poem to retrieve.</param>
public record GetPoemQuery(Guid PoemId) : IQuery<Poem?>;
