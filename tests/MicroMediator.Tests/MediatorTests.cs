using MicroMediator;
using Microsoft.Extensions.DependencyInjection;

namespace MicroMediator.Tests;

/// <summary>Unit tests for <see cref="IMediator"/> covering command dispatch, query dispatch, and handler independence.</summary>
public class MediatorTests
{
    #region Send

    /// <summary>
    /// Verifies that sending a command with a return value resolves the correct handler
    /// via the DI container and returns the expected result.
    /// </summary>
    [Fact]
    public async Task Send_CommandWithResult_ResolvesAndInvokesHandler_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ct = TestContext.Current.CancellationToken;
        var result = await mediator.Send(new TestCommand("hello"), ct);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that sending a void command (no return value) resolves and invokes
    /// the handler, observable through a side effect on the handler's static state.
    /// </summary>
    [Fact]
    public async Task Send_VoidCommand_ResolvesAndInvokesHandler_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestVoidCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        await mediator.Send(new TestVoidCommand(), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(TestVoidCommandHandler.Invoked);
    }

    /// <summary>
    /// Verifies that sending a query returns a correctly typed result computed by
    /// the matching query handler.
    /// </summary>
    [Fact]
    public async Task Send_Query_ResolvesAndInvokesHandler_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestQueryHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new TestQuery(42), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("42", result);
    }

    /// <summary>
    /// Verifies that multiple mediator instances from the same service provider
    /// produce independent handler invocations with no shared state interference.
    /// This confirms scoped service resolution.
    /// </summary>
    [Fact]
    public async Task Send_MultipleHandlers_AreIndependent_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();

        var mediator1 = provider.GetRequiredService<IMediator>();
        var mediator2 = provider.GetRequiredService<IMediator>();
        var ct = TestContext.Current.CancellationToken;

        // Act
        var r1 = await mediator1.Send(new TestCommand("hello"), ct);
        var r2 = await mediator2.Send(new TestCommand("hello"), ct);

        // Assert
        Assert.True(r1);
        Assert.True(r2);
    }

    /// <summary>
    /// Verifies that command and query handlers coexist in the same container
    /// without handler-type collisions. A single mediator dispatches both
    /// <c>ICommand&lt;TResult&gt;</c> and <c>IQuery&lt;TResult&gt;</c> messages
    /// to their respective handler implementations.
    /// </summary>
    [Fact]
    public async Task Send_DifferentHandlerTypes_AreIndependent_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var ct = TestContext.Current.CancellationToken;

        // Act
        var cmdResult = await mediator.Send(new TestCommand("hello"), ct);
        var queryResult = await mediator.Send(new TestQuery(99), ct);

        // Assert
        Assert.True(cmdResult);
        Assert.Equal("99", queryResult);
    }

    /// <summary>
    /// Verifies that sending a void command whose handler type has not been registered
    /// via <see cref="ServiceCollectionExtensions.AddMediator"/> throws an
    /// <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public async Task Send_VoidCommandNotRegistered_ThrowsInvalidOperationException_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new UnregisteredVoidCommand(), TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Contains("ICommandHandler", ex.Message);
    }

    /// <summary>
    /// Verifies that sending a command with a result whose handler has not been registered
    /// throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public async Task Send_CommandWithResultNotRegistered_ThrowsInvalidOperationException_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new UnregisteredResultCommand(), TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Contains("ICommandHandler", ex.Message);
    }

    /// <summary>
    /// Verifies that sending a query whose handler has not been registered
    /// throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public async Task Send_QueryNotRegistered_ThrowsInvalidOperationException_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new UnregisteredQuery(), TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Contains("IQueryHandler", ex.Message);
    }

    /// <summary>
    /// Verifies that the cancellation token passed to <see cref="IMediator.Send{TCommand}"/>
    /// for a void command is forwarded to the handler's <c>Handle</c> method unchanged.
    /// </summary>
    [Fact]
    public async Task Send_VoidCommand_PassesCancellationTokenToHandler_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(CancellationVoidCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        using var cts = new CancellationTokenSource();
        var handler = provider.GetRequiredService<ICommandHandler<CancellationVoidCommand>>();

        // Act
        await mediator.Send(new CancellationVoidCommand(), cts.Token);

        // Assert
        Assert.Equal(cts.Token, ((CancellationVoidCommandHandler)handler).CapturedCancellationToken);
    }

    /// <summary>
    /// Verifies that the cancellation token is forwarded to the handler for a command
    /// that returns a result.
    /// </summary>
    [Fact]
    public async Task Send_CommandWithResult_PassesCancellationTokenToHandler_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(CancellationResultCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        using var cts = new CancellationTokenSource();
        var handler = provider.GetRequiredService<ICommandHandler<CancellationResultCommand, bool>>();

        // Act
        await mediator.Send(new CancellationResultCommand(), cts.Token);

        // Assert
        Assert.Equal(cts.Token, ((CancellationResultCommandHandler)handler).CapturedCancellationToken);
    }

    /// <summary>
    /// Verifies that the cancellation token is forwarded to the handler for a query
    /// that returns a result.
    /// </summary>
    [Fact]
    public async Task Send_Query_PassesCancellationTokenToHandler_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(CancellationQueryHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        using var cts = new CancellationTokenSource();
        var handler = provider.GetRequiredService<IQueryHandler<CancellationQuery, string>>();

        // Act
        await mediator.Send(new CancellationQuery(), cts.Token);

        // Assert
        Assert.Equal(cts.Token, ((CancellationQueryHandler)handler).CapturedCancellationToken);
    }

    /// <summary>
    /// Verifies that an exception thrown by a void command handler propagates
    /// back to the caller of <see cref="IMediator.Send{TCommand}"/> unwrapped.
    /// </summary>
    [Fact]
    public async Task Send_VoidCommandHandlerThrows_ThrowsInvalidOperationException_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(ThrowingVoidCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new ThrowingVoidCommand(), TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Equal("Handler threw.", ex.Message);
    }

    /// <summary>
    /// Verifies that an exception thrown by a command-with-result handler propagates
    /// back to the caller unwrapped.
    /// </summary>
    [Fact]
    public async Task Send_CommandWithResultHandlerThrows_ThrowsInvalidOperationException_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(ThrowingResultCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new ThrowingResultCommand(), TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Equal("Handler threw.", ex.Message);
    }

    /// <summary>
    /// Verifies that an <see cref="OperationCanceledException"/> thrown by a handler
    /// propagates to the caller as an <see cref="OperationCanceledException"/>,
    /// not wrapped in <see cref="TaskCanceledException"/>.
    /// </summary>
    [Fact]
    public async Task Send_HandlerCancels_ThrowsOperationCanceledException_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(CanceledVoidCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act / Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            mediator.Send(new CanceledVoidCommand(), TestContext.Current.CancellationToken)
        );
    }

    /// <summary>
    /// Verifies that <see cref="Mediator.Send{TResult}(ICommand{TResult},CancellationToken)"/> can be invoked
    /// concurrently from many callers without cross-contamination, lost results, or threading exceptions.
    /// Each dispatch carries a unique index; the test confirms every index is returned once.
    /// </summary>
    [Fact]
    public async Task Send_ConcurrentDispatches_CompletesWithoutErrors_Test()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(ConcurrencyCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        const int count = 100;
        var tasks = Enumerable
            .Range(0, count)
            .Select(i => mediator.Send(new ConcurrencyCommand(i), TestContext.Current.CancellationToken))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        var handler = provider.GetRequiredService<ICommandHandler<ConcurrencyCommand, int>>();
        Assert.Equal(count, ((ConcurrencyCommandHandler)handler).CallCount);
        Assert.Equal(count, results.Length);
    }

    #endregion

    /// <summary>A test command returning <c>bool</c>, used to verify command dispatch.</summary>
    public record TestCommand(string Value) : ICommand<bool>;

    /// <summary>Handles <see cref="TestCommand"/> by validating the value equals <c>"hello"</c>.</summary>
    public class TestCommandHandler : ICommandHandler<TestCommand, bool>
    {
        public Task<bool> Handle(TestCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(command.Value == "hello");
        }
    }

    /// <summary>A test command with no return value, used to verify void command dispatch.</summary>
    public record TestVoidCommand : ICommand;

    /// <summary>Handles <see cref="TestVoidCommand"/> by setting <see cref="TestVoidCommandHandler.Invoked"/> to <c>true</c>.</summary>
    public class TestVoidCommandHandler : ICommandHandler<TestVoidCommand>
    {
        public static bool Invoked;

        public Task Handle(TestVoidCommand command, CancellationToken cancellationToken)
        {
            Invoked = true;
            return Task.CompletedTask;
        }
    }

    /// <summary>A test query returning <c>string</c>, used to verify query dispatch.</summary>
    public record TestQuery(int Number) : IQuery<string>;

    /// <summary>Handles <see cref="TestQuery"/> by converting the number to a string.</summary>
    public class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> Handle(TestQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(query.Number.ToString());
        }
    }

    /// <summary>A void command with no registered handler, used to verify error paths.</summary>
    public record UnregisteredVoidCommand : ICommand;

    /// <summary>A command with a result and no registered handler, used to verify error paths.</summary>
    public record UnregisteredResultCommand : ICommand<bool>;

    /// <summary>A query with no registered handler, used to verify error paths.</summary>
    public record UnregisteredQuery : IQuery<string>;

    /// <summary>A void command used to verify cancellation token propagation.</summary>
    public record CancellationVoidCommand : ICommand;

    /// <summary>Handles <see cref="CancellationVoidCommand"/> by capturing the received cancellation token.</summary>
    public class CancellationVoidCommandHandler : ICommandHandler<CancellationVoidCommand>
    {
        public CancellationToken CapturedCancellationToken { get; private set; }

        public Task Handle(CancellationVoidCommand command, CancellationToken cancellationToken)
        {
            CapturedCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }

    /// <summary>A command with a result used to verify cancellation token propagation.</summary>
    public record CancellationResultCommand : ICommand<bool>;

    /// <summary>Handles <see cref="CancellationResultCommand"/> by capturing the received cancellation token.</summary>
    public class CancellationResultCommandHandler : ICommandHandler<CancellationResultCommand, bool>
    {
        public CancellationToken CapturedCancellationToken { get; private set; }

        public Task<bool> Handle(CancellationResultCommand command, CancellationToken cancellationToken)
        {
            CapturedCancellationToken = cancellationToken;
            return Task.FromResult(true);
        }
    }

    /// <summary>A query used to verify cancellation token propagation.</summary>
    public record CancellationQuery : IQuery<string>;

    /// <summary>Handles <see cref="CancellationQuery"/> by capturing the received cancellation token.</summary>
    public class CancellationQueryHandler : IQueryHandler<CancellationQuery, string>
    {
        public CancellationToken CapturedCancellationToken { get; private set; }

        public Task<string> Handle(CancellationQuery query, CancellationToken cancellationToken)
        {
            CapturedCancellationToken = cancellationToken;
            return Task.FromResult("ok");
        }
    }

    /// <summary>A void command used to verify exception propagation from a handler.</summary>
    public record ThrowingVoidCommand : ICommand;

    /// <summary>Handles <see cref="ThrowingVoidCommand"/> by throwing an expected exception via a faulted task.</summary>
    public class ThrowingVoidCommandHandler : ICommandHandler<ThrowingVoidCommand>
    {
        public Task Handle(ThrowingVoidCommand command, CancellationToken cancellationToken)
        {
            return Task.FromException(new InvalidOperationException("Handler threw."));
        }
    }

    /// <summary>A command with a result used to verify exception propagation.</summary>
    public record ThrowingResultCommand : ICommand<string>;

    /// <summary>Handles <see cref="ThrowingResultCommand"/> by throwing via a faulted task.</summary>
    public class ThrowingResultCommandHandler : ICommandHandler<ThrowingResultCommand, string>
    {
        public Task<string> Handle(ThrowingResultCommand command, CancellationToken cancellationToken)
        {
            return Task.FromException<string>(new InvalidOperationException("Handler threw."));
        }
    }

    /// <summary>A void command used to verify OperationCanceledException propagation.</summary>
    public record CanceledVoidCommand : ICommand;

    /// <summary>Handles <see cref="CanceledVoidCommand"/> by throwing OperationCanceledException via a faulted task.</summary>
    public class CanceledVoidCommandHandler : ICommandHandler<CanceledVoidCommand>
    {
        public Task Handle(CanceledVoidCommand command, CancellationToken cancellationToken)
        {
            return Task.FromException(new OperationCanceledException());
        }
    }

    /// <summary>A command carrying a unique index, used to verify thread-safe concurrent dispatch.</summary>
    public record ConcurrencyCommand(int Index) : ICommand<int>;

    /// <summary>
    /// Handles <see cref="ConcurrencyCommand"/> by tracking call count with
    /// <see cref="Interlocked.Increment"/> and returning the command index.
    /// </summary>
    public class ConcurrencyCommandHandler : ICommandHandler<ConcurrencyCommand, int>
    {
        private int _callCount;

        public int CallCount => _callCount;

        public Task<int> Handle(ConcurrencyCommand command, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            return Task.FromResult(command.Index);
        }
    }
}
