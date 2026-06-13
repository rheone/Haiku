namespace MicroMediator;

/// <summary>
/// Dispatches commands and queries to their registered handlers.
/// </summary>
/// <remarks>
/// <para>
/// This interface follows the Mediator pattern and uses three <c>Send</c> overloads
/// to distinguish between commands with no return value, commands with a return value, and queries
/// with a return value. Overload resolution is driven by the concrete type passed at the call site.
/// </para>
/// </remarks>
public interface IMediator
{
    /// <summary>
    /// Sends a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command. Must implement <see cref="ICommand"/>.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for <typeparamref name="TCommand"/>.</exception>
    Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Sends a command that produces a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the command.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the command result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the command type.</exception>
    Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a query and returns its result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the query result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the query type.</exception>
    Task<TResult> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
