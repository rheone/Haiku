using Haiku.Domain.Entities;
using MicroMediator;

namespace Haiku.Services.Slices.PoetProfile;

/// <summary>
/// Query to retrieve a poet's profile by author identifier. Returns <c>null</c> when the user does not exist.
/// </summary>
/// <param name="AuthorId">The identifier of the author whose profile to retrieve.</param>
public record GetPoetProfileQuery(Guid AuthorId) : IQuery<User?>;
