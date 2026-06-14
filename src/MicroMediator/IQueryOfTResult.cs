namespace MicroMediator;

/// <summary>
/// Defines a marker interface for queries that return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the query.</typeparam>
/// <remarks>
/// <para>
/// Use this variant for all read operations that return domain data. The mediator dispatches to
/// <see cref="IQueryHandler{TQuery, TResult}"/> for queries implementing this interface.
/// </para>
/// </remarks>
public interface IQuery<TResult> { }
