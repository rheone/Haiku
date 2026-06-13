#pragma warning disable SA1402 // File may only contain a single type
namespace MicroMediator;

/// <summary>
/// Defines a marker interface for commands that do not return a result.
/// Use <see cref="ICommand{TResult}"/> for commands that produce a result.
/// </summary>
public interface ICommand { }

/// <summary>
/// Defines a marker interface for commands that return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the command.</typeparam>
public interface ICommand<TResult> { }
