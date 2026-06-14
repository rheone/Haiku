using Microsoft.Extensions.DependencyInjection;

namespace MicroMediator;

/// <summary>
/// Dispatches commands and queries by resolving handlers from the service provider using reflection.
/// </summary>
/// <remarks>
/// <para>
/// Handlers are resolved at runtime based on the concrete type of the command or query object.
/// Because only the abstract interface type is known at compile time (e.g., <see cref="ICommand"/>),
/// the implementation uses <see cref="System.Reflection"/> to construct the closed generic handler
/// type and invoke its <c>Handle</c> method. This approach avoids requiring a separate registration
/// call for every command/query type pair — the <see cref="ServiceCollectionExtensions.AddMediator"/>
/// method scans the assembly and registers all handlers automatically.
/// </para>
/// </remarks>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve registered handler instances.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for <typeparamref name="TCommand"/>.</exception>
    public async Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        // The generic constraint only exposes ICommand at compile time; reflect on the
        // concrete runtime type to construct the closed generic ICommandHandler<T>.
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

        // GetRequiredService throws InvalidOperationException if the handler
        // was not registered by AddMediator.
        var handler = _serviceProvider.GetRequiredService(handlerType);

        // Resolve the Handle(TCommand, CancellationToken) method by matching
        // the concrete parameter types — the null-forgiving operator is safe
        // because ICommandHandler<T> guarantees this method exists.
        var method = handlerType.GetMethod("Handle", [commandType, typeof(CancellationToken)])!;
        await (Task)method.Invoke(handler, [command, cancellationToken])!;
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the command type.</exception>
    public async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        // Same reflection dispatch as the void-command overload, but targeting
        // ICommandHandler<TCommand, TResult> to produce a typed result.
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle", [commandType, typeof(CancellationToken)])!;
        return await (Task<TResult>)method.Invoke(handler, [command, cancellationToken])!;
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the query type.</exception>
    public async Task<TResult> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        // Identical pattern to the command overloads, but resolving
        // IQueryHandler<TQuery, TResult> instead.
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle", [queryType, typeof(CancellationToken)])!;
        return await (Task<TResult>)method.Invoke(handler, [query, cancellationToken])!;
    }
}
