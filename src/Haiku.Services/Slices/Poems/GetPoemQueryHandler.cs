using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Handles retrieval of a single poem by its identifier.
/// </summary>
public class GetPoemQueryHandler : IQueryHandler<GetPoemQuery, Poem?>
{
    private readonly IPoemRepository _poemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPoemQueryHandler"/> class.
    /// </summary>
    /// <param name="poemRepository">Repository for loading <see cref="Poem"/> entities.</param>
    public GetPoemQueryHandler(IPoemRepository poemRepository)
    {
        _poemRepository = poemRepository;
    }

    /// <summary>
    /// Retrieves the poem matching the query's identifier.
    /// </summary>
    /// <param name="request">The query containing the poem identifier to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching <see cref="Poem"/> or <c>null</c> if not found.</returns>
    public async Task<Poem?> Handle(GetPoemQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _poemRepository.GetByIdAsync(request.PoemId, cancellationToken);
    }
}
