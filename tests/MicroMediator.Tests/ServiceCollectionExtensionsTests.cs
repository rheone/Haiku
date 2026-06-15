using MicroMediator;
using Microsoft.Extensions.DependencyInjection;

namespace MicroMediator.Tests;

/// <summary>Unit tests for <see cref="ServiceCollectionExtensions"/> covering mediator and handler registration.</summary>
public class ServiceCollectionExtensionsTests
{
    #region AddMediator

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddMediator"/> registers <see cref="IMediator"/>
    /// as a scoped service pointing to <see cref="Mediator"/> as the implementation.
    /// </summary>
    [Fact]
    public void AddMediator_IMediator_RegistersAsScoped_Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediator(typeof(VoidCmdHandler).Assembly);

        // Assert
        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IMediator));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(Mediator), descriptor.ImplementationType);
    }

    /// <summary>
    /// Verifies that a handler implementing <c>ICommandHandler&lt;TCommand&gt;</c> (void command handler)
    /// is discovered and registered, and can be resolved from the built service provider.
    /// </summary>
    [Fact]
    public void AddMediator_VoidCommandHandler_Registers_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(VoidCmdHandler).Assembly);
        var provider = services.BuildServiceProvider();

        // Act
        var handler = provider.GetRequiredService<ICommandHandler<VoidCmd>>();

        // Assert
        Assert.IsType<VoidCmdHandler>(handler);
    }

    /// <summary>
    /// Verifies that a handler implementing <c>ICommandHandler&lt;TCommand, TResult&gt;</c>
    /// (command that returns a result) is discovered and registered.
    /// </summary>
    [Fact]
    public void AddMediator_CommandWithResultHandler_Registers_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(ResultCmdHandler).Assembly);
        var provider = services.BuildServiceProvider();

        // Act
        var handler = provider.GetRequiredService<ICommandHandler<ResultCmd, bool>>();

        // Assert
        Assert.IsType<ResultCmdHandler>(handler);
    }

    /// <summary>
    /// Verifies that a handler implementing <c>IQueryHandler&lt;TQuery, TResult&gt;</c>
    /// (query returning a result) is discovered and registered.
    /// </summary>
    [Fact]
    public void AddMediator_QueryHandler_Registers_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(ResultQueryHandler).Assembly);
        var provider = services.BuildServiceProvider();

        // Act
        var handler = provider.GetRequiredService<IQueryHandler<ResultQuery, string>>();

        // Assert
        Assert.IsType<ResultQueryHandler>(handler);
    }

    /// <summary>
    /// Verifies that abstract handler classes are skipped during assembly scanning.
    /// Only the concrete handler for a command should be registered, not an abstract class
    /// that also implements the handler interface.
    /// </summary>
    [Fact]
    public void AddMediator_AbstractHandlers_Skips_Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediator(typeof(AbstractOnlyCmdHandler).Assembly);

        // Assert
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICommandHandler<AbstractOnlyCmd>));
    }

    /// <summary>
    /// Verifies that interface types are skipped during assembly scanning.
    /// An interface that extends a handler interface should not be registered
    /// as a handler implementation since it cannot be instantiated.
    /// </summary>
    [Fact]
    public void AddMediator_InterfaceTypes_Skips_Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediator(typeof(IInterfaceOnlyCmdHandler).Assembly);

        // Assert
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICommandHandler<InterfaceOnlyCmd>));
    }

    /// <summary>
    /// Verifies that scanning an assembly with no handler implementations does not throw
    /// and returns the service collection for fluent chaining.
    /// </summary>
    [Fact]
    public void AddMediator_EmptyAssembly_HandlesGracefully_Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMediator(typeof(string).Assembly);

        // Assert
        Assert.Same(services, result);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddMediator"/> returns the same
    /// <see cref="IServiceCollection"/> instance for fluent method chaining.
    /// </summary>
    [Fact]
    public void AddMediator_ServiceCollection_ReturnsForChaining_Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMediator(typeof(VoidCmdHandler).Assembly);

        // Assert
        Assert.Same(services, result);
    }

    /// <summary>
    /// Verifies that a single handler class implementing multiple handler interfaces
    /// (e.g. both <c>ICommandHandler&lt;T&gt;</c> and <c>IQueryHandler&lt;TQ, TR&gt;</c>)
    /// is registered for each implemented handler service interface independently.
    /// </summary>
    [Fact]
    public void AddMediator_MultipleInterfaces_RegistersEachIndependently_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(MultiHandler).Assembly);
        var provider = services.BuildServiceProvider();

        // Act
        var commandHandler = provider.GetRequiredService<ICommandHandler<MultiCmd>>();
        var queryHandler = provider.GetRequiredService<IQueryHandler<MultiQuery, int>>();

        // Assert
        Assert.IsType<MultiHandler>(commandHandler);
        Assert.IsType<MultiHandler>(queryHandler);
    }

    /// <summary>
    /// Verifies that non-public handler types are not discovered by <c>GetExportedTypes()</c>
    /// and therefore not registered. An internal handler class should be invisible to the scanner.
    /// </summary>
    [Fact]
    public void AddMediator_NonPublicHandlers_Skips_Test()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediator(typeof(InternalOnlyCmd).Assembly);

        // Assert
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICommandHandler<InternalOnlyCmd>));
    }

    #endregion

    /// <summary>A void command used to verify void command handler registration.</summary>
    public record VoidCmd : ICommand;

    /// <summary>Concrete handler for <see cref="VoidCmd"/>.</summary>
    public class VoidCmdHandler : ICommandHandler<VoidCmd>
    {
        public Task Handle(VoidCmd command, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    /// <summary>A command returning a result, used to verify result-command handler registration.</summary>
    public record ResultCmd : ICommand<bool>;

    /// <summary>Concrete handler for <see cref="ResultCmd"/>.</summary>
    public class ResultCmdHandler : ICommandHandler<ResultCmd, bool>
    {
        public Task<bool> Handle(ResultCmd command, CancellationToken cancellationToken) => Task.FromResult(true);
    }

    /// <summary>A query returning a result, used to verify query handler registration.</summary>
    public record ResultQuery : IQuery<string>;

    /// <summary>Concrete handler for <see cref="ResultQuery"/>.</summary>
    public class ResultQueryHandler : IQueryHandler<ResultQuery, string>
    {
        public Task<string> Handle(ResultQuery query, CancellationToken cancellationToken) => Task.FromResult("ok");
    }

    /// <summary>A command whose only handler candidate is abstract — used to verify abstract handlers are skipped.</summary>
    public record AbstractOnlyCmd : ICommand;

    /// <summary>Abstract handler for <see cref="AbstractOnlyCmd"/> — should not be registered.</summary>
    public abstract class AbstractOnlyCmdHandler : ICommandHandler<AbstractOnlyCmd>
    {
        public abstract Task Handle(AbstractOnlyCmd command, CancellationToken cancellationToken);
    }

    /// <summary>A command whose only handler candidate is an interface — used to verify interfaces are skipped.</summary>
    public record InterfaceOnlyCmd : ICommand;

    /// <summary>Interface handler for <see cref="InterfaceOnlyCmd"/> — should not be registered.</summary>
    public interface IInterfaceOnlyCmdHandler : ICommandHandler<InterfaceOnlyCmd>;

    /// <summary>A command used to verify multi-interface handler registration.</summary>
    public record MultiCmd : ICommand;

    /// <summary>A query used to verify multi-interface handler registration.</summary>
    public record MultiQuery : IQuery<int>;

    /// <summary>Handler implementing two handler interfaces — both should be registered.</summary>
    public class MultiHandler : ICommandHandler<MultiCmd>, IQueryHandler<MultiQuery, int>
    {
        public Task Handle(MultiCmd command, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<int> Handle(MultiQuery query, CancellationToken cancellationToken) => Task.FromResult(0);
    }

    /// <summary>A command whose only handler is internal — used to verify non-public handlers are skipped.</summary>
    public record InternalOnlyCmd : ICommand;

    /// <summary>Internal handler for <see cref="InternalOnlyCmd"/> — should not be discovered by GetExportedTypes().</summary>
    internal class InternalOnlyCmdHandler : ICommandHandler<InternalOnlyCmd>
    {
        public Task Handle(InternalOnlyCmd command, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
