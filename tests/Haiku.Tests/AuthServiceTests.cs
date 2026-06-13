using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Haiku.Services.Auth;
using NSubstitute;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="AuthService"/> covering registration and login flows.</summary>
public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_CreatesUser_WhenEmailAndUsernameAreUnique()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);
        userRepo.UsernameExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);

        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync(
            "test@example.com",
            "testuser",
            "password123",
            TestContext.Current.CancellationToken
        );

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("testuser", result.Username);
        Assert.NotEqual("password123", result.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenEmailExists()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync("existing@example.com", TestContext.Current.CancellationToken).Returns(true);

        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync(
            "existing@example.com",
            "newuser",
            "password123",
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenUsernameExists()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);
        userRepo.UsernameExistsAsync("taken", TestContext.Current.CancellationToken).Returns(true);

        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync(
            "new@example.com",
            "taken",
            "password123",
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsUser_WhenCredentialsAreValid()
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

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync(
            "test@example.com",
            "correct_password",
            TestContext.Current.CancellationToken
        );

        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordIsWrong()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12),
        };
        userRepo.GetByEmailAsync("test@example.com", TestContext.Current.CancellationToken).Returns(user);

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("test@example.com", "wrong_password", TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenEmailDoesNotExist()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByEmailAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns((User?)null);

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("nonexistent@example.com", "password", TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserIsDisabled()
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

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("disabled@example.com", "password", TestContext.Current.CancellationToken);

        Assert.Null(result);
    }
}
