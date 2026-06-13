#pragma warning disable SA1402 // File may only contain a single type
namespace MicroMediator;

/// <summary>
/// Defines a marker interface for queries that do not return a result.
/// Use <see cref="IQuery{TResult}"/> for queries that produce a result.
/// </summary>
public interface IQuery { }

/// <summary>
/// Defines a marker interface for queries that return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the query.</typeparam>
public interface IQuery<TResult> { }
