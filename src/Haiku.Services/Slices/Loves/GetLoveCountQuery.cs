using MicroMediator;

namespace Haiku.Services.Slices.Loves;

/// <summary>
/// Query to retrieve the total number of loves (likes) for a poem.
/// </summary>
/// <param name="PoemId">The identifier of the poem.</param>
public record GetLoveCountQuery(Guid PoemId) : IQuery<int>;
