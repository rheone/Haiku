using MicroMediator;

namespace Haiku.Modules.Moderation.Queries;

/// <summary>
/// Query to check whether a user has a specific moderation privilege.
/// </summary>
/// <param name="UserId">The identifier of the user.</param>
/// <param name="Privilege">The privilege to check (e.g., "moderate", "admin").</param>
public record HasPrivilegeQuery(Guid UserId, string Privilege) : IQuery<bool>;
