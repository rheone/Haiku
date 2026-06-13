using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Moderation;

/// <summary>
/// Handles moderation privilege checks against the database.
/// </summary>
public class HasPrivilegeQueryHandler : IQueryHandler<HasPrivilegeQuery, bool>
{
    private readonly IModerationRepository _moderationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="HasPrivilegeQueryHandler"/> class.
    /// </summary>
    /// <param name="moderationRepository">The moderation repository for data access.</param>
    public HasPrivilegeQueryHandler(IModerationRepository moderationRepository)
    {
        _moderationRepository = moderationRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(HasPrivilegeQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _moderationRepository.HasPrivilegeAsync(request.UserId, request.Privilege, cancellationToken);
    }
}
