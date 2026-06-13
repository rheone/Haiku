using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Haiku.Services.Slices.Auth;
using NSubstitute;

namespace Haiku.Tests.Slices.Auth;

/// <summary>Unit tests for <see cref="LoginUserQueryHandler"/> covering credential validation, missing accounts, and disabled users.</summary>
public class LoginUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUser_WhenCredentialsAreValid()
    {
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
        var result = await handler.Handle(
            new LoginUserQuery("test@example.com", "correct_password"),
            TestContext.Current.CancellationToken
        );

        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenPasswordIsWrong()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12),
        };
        userRepo.GetByEmailAsync("test@example.com", TestContext.Current.CancellationToken).Returns(user);

        var handler = new LoginUserQueryHandler(userRepo);
        var result = await handler.Handle(
            new LoginUserQuery("test@example.com", "wrong_password"),
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenEmailDoesNotExist()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByEmailAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns((User?)null);

        var handler = new LoginUserQueryHandler(userRepo);
        var result = await handler.Handle(
            new LoginUserQuery("nonexistent@example.com", "password"),
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenUserIsDisabled()
    {
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
        var result = await handler.Handle(
            new LoginUserQuery("disabled@example.com", "password"),
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }
}
