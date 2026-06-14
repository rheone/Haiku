namespace MicroMediator;

/// <summary>
/// Defines a handler for queries that do not return a result.
/// </summary>
/// <typeparam name="TQuery">The concrete query type. Must implement <see cref="IQuery"/>.</typeparam>
/// <remarks>
/// <para>
/// Implement this interface to process a read-only query that performs no side effects.
/// The handler is resolved by the <see cref="IMediator"/> via the closed generic type
/// <c>IQueryHandler{TQuery}</c> at runtime.
/// </para>
/// </remarks>
public interface IQueryHandler<TQuery>
    where TQuery : IQuery
{
    /// <summary>
    /// Processes the specified query.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(TQuery query, CancellationToken cancellationToken);
}
