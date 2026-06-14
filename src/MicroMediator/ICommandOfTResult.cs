namespace MicroMediator;

/// <summary>
/// Defines a marker interface for commands that return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the command.</typeparam>
/// <remarks>
/// <para>
/// Use this variant when a command must return data (for example, the identifier of a newly created
/// entity) while still representing a state-changing operation. The mediator dispatches to
/// <see cref="ICommandHandler{TCommand, TResult}"/> for commands implementing this interface.
/// </para>
/// </remarks>
public interface ICommand<TResult> { }
