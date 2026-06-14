using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Haiku.Services.Slices.Auth;
using NSubstitute;

namespace Haiku.Tests.Slices.Auth;

/// <summary>Unit tests for <see cref="LoginUserQueryHandler"/> covering credential validation, missing accounts, and disabled users.</summary>
public class LoginUserQueryHandlerTests
{
    #region Handle

    /// <summary>Verifies that valid credentials return the authenticated user.</summary>
    [Fact]
    public async Task Handle_ReturnsUser_WhenCredentialsAreValid_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12),
            IsDisabled = false,
        };
        userRepo.GetByEmailAsync("test@example.com", TestContext.Current.CancellationToken).Returns(user);

        var handler = new LoginUserQueryHandler(userRepo);

        // Act
        var result = await handler.Handle(
            new LoginUserQuery("test@example.com", "correct_password"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    /// <summary>Verifies that an incorrect password returns null.</summary>
    [Fact]
    public async Task Handle_ReturnsNull_WhenPasswordIsWrong_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12),
        };
        userRepo.GetByEmailAsync("test@example.com", TestContext.Current.CancellationToken).Returns(user);

        var handler = new LoginUserQueryHandler(userRepo);

        // Act
        var result = await handler.Handle(
            new LoginUserQuery("test@example.com", "wrong_password"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that a non-existent email returns null.</summary>
    [Fact]
    public async Task Handle_ReturnsNull_WhenEmailDoesNotExist_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByEmailAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns((User?)null);

        var handler = new LoginUserQueryHandler(userRepo);

        // Act
        var result = await handler.Handle(
            new LoginUserQuery("nonexistent@example.com", "password"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that a disabled user returns null.</summary>
    [Fact]
    public async Task Handle_ReturnsNull_WhenUserIsDisabled_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "disabled@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password", workFactor: 12),
            IsDisabled = true,
        };
        userRepo.GetByEmailAsync("disabled@example.com", TestContext.Current.CancellationToken).Returns(user);

        var handler = new LoginUserQueryHandler(userRepo);

        // Act
        var result = await handler.Handle(
            new LoginUserQuery("disabled@example.com", "password"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that a deleted user returns null.</summary>
    /// <remarks>Auto Generated, verify expected behavior:</remarks>
    [Fact]
    public async Task Handle_ReturnsNull_WhenUserIsDeleted_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "deleted@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password", workFactor: 12),
            DeletedAt = DateTime.UtcNow,
        };
        userRepo.GetByEmailAsync("deleted@example.com", TestContext.Current.CancellationToken).Returns(user);

        var handler = new LoginUserQueryHandler(userRepo);

        // Act
        var result = await handler.Handle(
            new LoginUserQuery("deleted@example.com", "password"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that a cancelled token throws <see cref="OperationCanceledException"/>.</summary>
    /// <remarks>Auto Generated, verify expected behavior:</remarks>
    [Fact]
    public async Task Handle_ThrowsOperationCanceledException_WhenCancelled_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new LoginUserQueryHandler(userRepo);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(
                new LoginUserQuery("test@example.com", "password"),
                cts.Token
            )
        );
    }

    #endregion Handle
}
