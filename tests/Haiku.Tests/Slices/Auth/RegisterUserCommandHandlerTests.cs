using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Haiku.Services.Slices.Auth;
using NSubstitute;

namespace Haiku.Tests.Slices.Auth;

/// <summary>Unit tests for <see cref="RegisterUserCommandHandler"/> covering unique constraint enforcement during registration.</summary>
/// <remarks>
/// <para>
/// The handler returns <c>null</c> when either the email or username already exists, and
/// returns a new <see cref="User"/> entity with a hashed password when both are unique.
/// These tests verify only uniqueness constraints; password strength validation is tested
/// separately in the validation layer.
/// </para>
/// </remarks>
public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesUser_WhenEmailAndUsernameAreUnique()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);
        userRepo.UsernameExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);

        var handler = new RegisterUserCommandHandler(userRepo);
        var result = await handler.Handle(
            new RegisterUserCommand("test@example.com", "testuser", "password123"),
            TestContext.Current.CancellationToken
        );

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("testuser", result.Username);
        // Verifies that BCrypt hashing occurred (hash differs from the plaintext password).
        Assert.NotEqual("password123", result.PasswordHash);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenEmailExists()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync("existing@example.com", TestContext.Current.CancellationToken).Returns(true);

        var handler = new RegisterUserCommandHandler(userRepo);
        var result = await handler.Handle(
            new RegisterUserCommand("existing@example.com", "newuser", "password123"),
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenUsernameExists()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);
        userRepo.UsernameExistsAsync("taken", TestContext.Current.CancellationToken).Returns(true);

        var handler = new RegisterUserCommandHandler(userRepo);
        var result = await handler.Handle(
            new RegisterUserCommand("new@example.com", "taken", "password123"),
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }
}
