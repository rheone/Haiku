using MicroMediator;

namespace Haiku.Modules.WordSearch.Queries;

/// <summary>
/// Handles word-based poem search.
/// </summary>
/// <remarks>
/// <para>This handler currently returns an empty list. Full-text search implementation is pending.</para>
/// </remarks>
public class SearchPoemsByWordQueryHandler : IQueryHandler<SearchPoemsByWordQuery, List<Poem>>
{
    private readonly IPoemRepository _poemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPoemsByWordQueryHandler"/> class.
    /// </summary>
    /// <param name="poemRepository">Repository for searching <see cref="Poem"/> entities.</param>
    public SearchPoemsByWordQueryHandler(IPoemRepository poemRepository)
    {
        _poemRepository = poemRepository;
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
