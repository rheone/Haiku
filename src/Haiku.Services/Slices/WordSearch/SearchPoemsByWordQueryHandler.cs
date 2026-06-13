using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.WordSearch;

/// <summary>
/// Handles word-based poem search.
/// </summary>
/// <remarks>
/// <para>This handler currently returns an empty list. Full-text search implementation is pending.</para>
/// </remarks>
public class SearchPoemsByWordQueryHandler : IQueryHandler<SearchPoemsByWordQuery, List<Poem>>
{
    private readonly IHaikuRepository _haikuRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPoemsByWordQueryHandler"/> class.
    /// </summary>
    /// <param name="haikuRepository">Repository for searching <see cref="Poem"/> entities.</param>
    public SearchPoemsByWordQueryHandler(IHaikuRepository haikuRepository)
    {
        _haikuRepository = haikuRepository;
    }

    /// <summary>
    /// Executes a word-based poem search. Currently returns an empty list.
    /// </summary>
    /// <param name="request">The query containing the search word.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of matching poems.</returns>
    /// <remarks>
    /// <para>This is a stub implementation. Full-text search has not yet been wired to the repository.</para>
    /// </remarks>
    public async Task<List<Poem>> Handle(SearchPoemsByWordQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.FromResult(new List<Poem>());
    }
}
