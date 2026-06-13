namespace MicroMediator;

/// <summary>
/// Defines a handler for commands that do not return a result.
/// </summary>
/// <typeparam name="TCommand">The concrete command type. Must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Processes the specified command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(TCommand command, CancellationToken cancellationToken);
}
