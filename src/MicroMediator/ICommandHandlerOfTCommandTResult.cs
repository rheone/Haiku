namespace MicroMediator;

/// <summary>
/// Defines a handler for commands that return a result.
/// </summary>
/// <typeparam name="TCommand">The concrete command type. Must implement <see cref="ICommand{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of the result produced by handling the command.</typeparam>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Processes the specified command and returns a result.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the command result.</returns>
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}
