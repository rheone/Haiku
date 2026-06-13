using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Loves;

/// <summary>
/// Handles retrieving the love count for a poem from the database.
/// </summary>
public class GetLoveCountQueryHandler : IQueryHandler<GetLoveCountQuery, int>
{
    private readonly ILoveRepository _loveRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLoveCountQueryHandler"/> class.
    /// </summary>
    /// <param name="loveRepository">The love repository for data access.</param>
    public GetLoveCountQueryHandler(ILoveRepository loveRepository)
    {
        _loveRepository = loveRepository;
    }

    /// <inheritdoc/>
    public async Task<int> Handle(GetLoveCountQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _loveRepository.GetLoveCountAsync(request.PoemId, cancellationToken);
    }
}
