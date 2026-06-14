namespace MicroMediator;

/// <summary>
/// Defines a handler for commands that do not return a result.
/// </summary>
/// <typeparam name="TCommand">The concrete command type. Must implement <see cref="ICommand"/>.</typeparam>
/// <remarks>
/// <para>
/// Implement this interface to process a command that performs a side effect without
/// producing a result. The handler is resolved by the <see cref="IMediator"/> via
/// the closed generic type <c>ICommandHandler{TCommand}</c> at runtime.
/// </para>
/// </remarks>
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
