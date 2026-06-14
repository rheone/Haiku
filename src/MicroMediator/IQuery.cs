namespace MicroMediator;

/// <summary>
/// Defines a marker interface for queries that do not return a result.
/// Use <see cref="IQuery{TResult}"/> for queries that produce a result.
/// </summary>
/// <remarks>
/// <para>
/// In the MicroMediator CQRS pipeline, a query represents a read-only operation
/// that does not change system state. Unlike commands, queries are idempotent.
/// This marker interface enables the mediator to resolve the correct handler at runtime.
/// </para>
/// </remarks>
public interface IQuery { }
