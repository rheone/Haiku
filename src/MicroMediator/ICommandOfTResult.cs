namespace MicroMediator;

/// <summary>
/// Defines a marker interface for commands that return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the command.</typeparam>
public interface ICommand<TResult> { }
