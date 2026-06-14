namespace MicroMediator;

/// <summary>
/// Defines a marker interface for commands that do not return a result.
/// Use <see cref="ICommand{TResult}"/> for commands that produce a result.
/// </summary>
/// <remarks>
/// <para>
/// In the MicroMediator CQRS pipeline, a command represents an imperative operation
/// that changes system state. Unlike queries, commands are not idempotent and should
/// not return domain data. This marker interface enables the mediator to resolve
/// the correct <see cref="ICommandHandler{TCommand}"/> at runtime.
/// </para>
/// </remarks>
public interface ICommand { }
