using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Haiku.Domain.Interfaces;
using Haiku.Services.Auth;
using Haiku.Services.Haiku;
using NSubstitute;

namespace Haiku.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_CreatesUser_WhenEmailAndUsernameAreUnique()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>()).Returns(false);
        userRepo.UsernameExistsAsync(Arg.Any<string>()).Returns(false);

        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync("test@example.com", "testuser", "password123");

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("testuser", result.Username);
        Assert.NotEqual("password123", result.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenEmailExists()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync("existing@example.com").Returns(true);

        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync("existing@example.com", "newuser", "password123");

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenUsernameExists()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.EmailExistsAsync(Arg.Any<string>()).Returns(false);
        userRepo.UsernameExistsAsync("taken").Returns(true);

        var authService = new AuthService(userRepo);
        var result = await authService.RegisterAsync("new@example.com", "taken", "password123");

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
            IsDisabled = false
        };
        userRepo.GetByEmailAsync("test@example.com").Returns(user);

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("test@example.com", "correct_password");

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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password", workFactor: 12)
        };
        userRepo.GetByEmailAsync("test@example.com").Returns(user);

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("test@example.com", "wrong_password");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenEmailDoesNotExist()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("nonexistent@example.com", "password");

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
            IsDisabled = true
        };
        userRepo.GetByEmailAsync("disabled@example.com").Returns(user);

        var authService = new AuthService(userRepo);
        var result = await authService.LoginAsync("disabled@example.com", "password");

        Assert.Null(result);
    }
}

public class HaikuServiceTests
{
    [Fact]
    public void DetectPoemType_ReturnsHaiku_For575()
    {
        var content = "line one\nline two\nline three";
        var counts = new List<int> { 5, 7, 5 };

        var result = HaikuService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Haiku, result);
    }

    [Fact]
    public void DetectPoemType_ReturnsTanka_For57577()
    {
        var content = "1\n2\n3\n4\n5";
        var counts = new List<int> { 5, 7, 5, 7, 7 };

        var result = HaikuService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Tanka, result);
    }

    [Fact]
    public void DetectPoemType_ReturnsMonoku_ForSingleLine()
    {
        var content = "A single line of text";
        var counts = new List<int> { 7 };

        var result = HaikuService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public void DetectPoemType_ReturnsFreeform_WhenNoPatternMatches()
    {
        var content = "line one\nline two";
        var counts = new List<int> { 3, 9 };

        var result = HaikuService.DetectPoemType(content, counts);

        Assert.Equal(PoemType.Freeform, result);
    }

    [Fact]
    public void ExtractTags_ReturnsDistinctLowercaseTags()
    {
        var content = "This is #Nature at its #best #Nature";

        var tags = HaikuService.ExtractTags(content);

        Assert.Equal(2, tags.Count);
        Assert.Contains("nature", tags);
        Assert.Contains("best", tags);
    }

    [Fact]
    public void ExtractTags_ReturnsEmpty_WhenNoHashtags()
    {
        var content = "This has no tags at all";

        var tags = HaikuService.ExtractTags(content);

        Assert.Empty(tags);
    }
}

public class SyllableEngineTests
{
    [Fact]
    public void CountWordSyllables_ReturnsCustomDictionaryCount_WhenWordExists()
    {
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "haiku", 2 }
        };
        var engine = new SyllableEngine(customDict);

        var result = engine.CountWordSyllables("haiku");

        Assert.Equal(2, result);
    }

    [Fact]
    public void CountWordSyllables_ReturnsAtLeastOne_ForUnknownWords()
    {
        var engine = new SyllableEngine();

        var result = engine.CountWordSyllables("xyzzy");

        Assert.True(result >= 1);
    }

    [Fact]
    public void CountLineSyllables_ReturnsCountsForEachWord()
    {
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "the", 1 },
            { "silence", 2 },
            { "falls", 1 }
        };
        var engine = new SyllableEngine(customDict);

        var result = engine.CountLineSyllables("the silence falls");

        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 1, 2, 1 }, result);
    }

    [Fact]
    public void CountWordSyllables_StripsPunctuation()
    {
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "hello", 2 }
        };
        var engine = new SyllableEngine(customDict);

        Assert.Equal(2, engine.CountWordSyllables("hello!"));
        Assert.Equal(2, engine.CountWordSyllables("hello,"));
        Assert.Equal(2, engine.CountWordSyllables("\"hello\""));
    }
}

public class EmailServiceTests
{
    [Fact]
    public async Task SendVerificationEmailAsync_CallsEmailSender()
    {
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);

        await emailService.SendVerificationEmailAsync("test@example.com", "https://example.com/verify?token=abc");

        await sender.Received(1).SendEmailAsync(
            "test@example.com",
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_CallsEmailSender()
    {
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);

        await emailService.SendPasswordResetEmailAsync("test@example.com", "https://example.com/reset?token=abc");

        await sender.Received(1).SendEmailAsync(
            "test@example.com",
            Arg.Any<string>(),
            Arg.Any<string>());
    }
}
