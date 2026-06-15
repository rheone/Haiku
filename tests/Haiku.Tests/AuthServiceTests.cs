using NSubstitute;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="AuthService"/> covering registration and login flows.</summary>
/// <remarks>
/// <para>
/// Test fixtures use BCrypt work factor 12 for password hashing, matching the production
/// configuration. Each hash invocation adds approximately 300ms to test execution. When
/// adding new credential-validation tests, prefer unit tests against the repository mock
/// over integration tests to keep the feedback loop fast.
/// </para>
/// </remarks>
public class AuthServiceTests
{
    #region Register

    /// <summary>Verifies that RegisterAsync creates a user and returns it when the email and username are both unique.</summary>
    [Fact]
    public async Task RegisterAsync_CreatesUser_WhenEmailAndUsernameAreUnique()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);
        userRepo.UsernameExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync(
            "test@example.com",
            "testuser",
            "password123",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("testuser", result.Username);
        Assert.NotEqual("password123", result.PasswordHash);
    }

    /// <summary>Verifies that RegisterAsync returns null when the email is already taken.</summary>
    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenEmailExists()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync("existing@example.com", TestContext.Current.CancellationToken).Returns(true);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync(
            "existing@example.com",
            "newuser",
            "password123",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that RegisterAsync returns null when the username is already taken.</summary>
    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenUsernameExists()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns(false);
        userRepo.UsernameExistsAsync("taken", TestContext.Current.CancellationToken).Returns(true);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync(
            "new@example.com",
            "taken",
            "password123",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Null(result);
    }

    // Auto Generated, verify expected behavior:
    /// <summary>Verifies that RegisterAsync throws OperationCanceledException when the cancellation token is cancelled before processing begins.</summary>
    [Fact]
    public async Task RegisterAsync_CancelledToken_ThrowsOperationCanceledException_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var authService = new AuthService(userRepo);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            authService.RegisterAsync("test@example.com", "testuser", "password123", cts.Token)
        );
    }

    #endregion

    #region Login

    /// <summary>Verifies that LoginAsync returns the user when the email and password match and the account is active.</summary>
    [Fact]
    public async Task LoginAsync_ReturnsUser_WhenCredentialsAreValid()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            // Work factor 12 matches production cost; each hash takes ~300ms.
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12),
            IsDisabled = false,
        };
        userRepo.GetByEmailAsync("test@example.com", TestContext.Current.CancellationToken).Returns(user);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync(
            "test@example.com",
            "correct_password",
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    /// <summary>Verifies that LoginAsync returns null when the password does not match.</summary>
    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordIsWrong()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            // Work factor 12 matches production cost; each hash takes ~300ms.
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12),
        };
        userRepo.GetByEmailAsync("test@example.com", TestContext.Current.CancellationToken).Returns(user);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("test@example.com", "wrong_password", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that LoginAsync returns null when the email address is not registered.</summary>
    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenEmailDoesNotExist()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByEmailAsync(Arg.Any<string>(), TestContext.Current.CancellationToken).Returns((User?)null);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("nonexistent@example.com", "password", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>Verifies that LoginAsync returns null when the user account has been disabled.</summary>
    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserIsDisabled()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "disabled@example.com",
            // Work factor 12 matches production cost; each hash takes ~300ms.
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password", workFactor: 12),
            IsDisabled = true,
        };
        userRepo.GetByEmailAsync("disabled@example.com", TestContext.Current.CancellationToken).Returns(user);

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("disabled@example.com", "password", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    // Auto Generated, verify expected behavior:
    /// <summary>Verifies that LoginAsync returns null when the user account has been soft-deleted (DeletedAt is set).</summary>
    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserIsDeleted_Test()
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

        // Act
        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("deleted@example.com", "password", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    // Auto Generated, verify expected behavior:
    /// <summary>Verifies that LoginAsync throws OperationCanceledException when the cancellation token is cancelled before processing begins.</summary>
    [Fact]
    public async Task LoginAsync_CancelledToken_ThrowsOperationCanceledException_Test()
    {
        // Arrange
        var userRepo = Substitute.For<IUserRepository>();
        var authService = new AuthService(userRepo);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            authService.LoginAsync("test@example.com", "password", cts.Token)
        );
    }

    #endregion
}
