namespace MicroMediator;

/// <summary>
/// Defines a handler for queries that return a result.
/// </summary>
/// <typeparam name="TQuery">The concrete query type. Must implement <see cref="IQuery{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of the result produced by handling the query.</typeparam>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Processes the specified query and returns a result.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the query result.</returns>
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}
