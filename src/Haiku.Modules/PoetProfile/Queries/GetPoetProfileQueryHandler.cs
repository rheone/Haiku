using MicroMediator;

namespace Haiku.Modules.PoetProfile.Queries;

/// <summary>
/// Handles retrieval of a poet's user profile by author identifier.
/// </summary>
public class GetPoetProfileQueryHandler : IQueryHandler<GetPoetProfileQuery, User?>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPoetProfileQueryHandler"/> class.
    /// </summary>
    /// <param name="userRepository">Repository for loading <see cref="User"/> entities.</param>
    public GetPoetProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Retrieves the user profile matching the query's author identifier.
    /// </summary>
    /// <param name="request">The query containing the author identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching <see cref="User"/> profile or <c>null</c> if not found.</returns>
    public async Task<User?> Handle(GetPoetProfileQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
    }
}
