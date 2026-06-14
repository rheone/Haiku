using MicroMediator;
using Microsoft.Extensions.DependencyInjection;

namespace MicroMediator.Tests;

/// <summary>Unit tests for <see cref="IMediator"/> covering command dispatch, query dispatch, and handler independence.</summary>
public class MediatorTests
{
    #region Send

    /// <summary>Sending a command with a result resolves the correct handler and returns the expected value.</summary>
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

    /// <summary>Sending a void command resolves the correct handler and sets the invocation flag.</summary>
    [Fact]
    public async Task Send_VoidCommand_ResolvesAndInvokesHandler_Test()
    {
        // Arrange
        TestVoidCommandHandler.Invoked = false;
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestVoidCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        await mediator.Send(new TestVoidCommand(), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(TestVoidCommandHandler.Invoked);
    }

    /// <summary>Sending a query resolves the correct handler and returns the expected transformed result.</summary>
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

    /// <summary>Multiple mediator instances are independent — each resolves and invokes its own handler.</summary>
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

    /// <summary>Different handler types (command vs query) are independent and both resolve correctly from the same mediator.</summary>
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

    /// <summary>Handles <see cref="TestVoidCommand"/> by setting <see cref="Invoked"/> to <c>true</c>.</summary>
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
}
