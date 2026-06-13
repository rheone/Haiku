using System.Security.Claims;
using Haiku.Domain.Interfaces;
using Haiku.Infrastructure;
using Haiku.Infrastructure.Email;
using Haiku.Infrastructure.Repositories;
using Haiku.Services;
using Haiku.Services.Auth;
using Haiku.Services.Dictionary;
using Haiku.Services.Haiku;
using Haiku.Services.Moderation;
using Haiku.Services.Poems;
using Haiku.Services.Poems.Matchers;
using Haiku.Services.Slices.Auth;
using Haiku.Web.Components;
using Haiku.Web.Middleware;
using MicroMediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Exceptional;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddExceptional();

// Connection string falls back to a local SQL Server for development.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=Haiku;Trusted_Connection=True;TrustServerCertificate=True;";

// EF Core: scoped DbContext per request.
builder.Services.AddDbContext<HaikuDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3))
);

// Repositories — scoped per request.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHaikuRepository, HaikuRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<ILoveRepository, LoveRepository>();
builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();
builder.Services.AddScoped<IModerationRepository, ModerationRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Application services — scoped per request (AuthService, HaikuService, etc.).
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HaikuService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ModerationService>();
builder.Services.AddScoped<DictionaryService>();

// SyllableEngine and PoemEngine hold the CMU dictionary in memory; shared across all requests.
builder.Services.AddSingleton<SyllableEngine>();
builder.Services.AddSingleton<PoemEngine>();

// Poem input processing and type detection
builder.Services.AddScoped<IPoemInputService, PoemInputService>();
builder.Services.AddScoped<IPoemMatcherChain, PoemMatcherChain>();
builder.Services.AddScoped<IPoemMatcher, MonokuMatcher>();
builder.Services.AddScoped<IPoemMatcher, HaikuMatcher>();
builder.Services.AddScoped<IPoemMatcher, KatautaMatcher>();
builder.Services.AddScoped<IPoemMatcher, AmericanLuneMatcher>();
builder.Services.AddScoped<IPoemMatcher, KellyLuneMatcher>();
builder.Services.AddScoped<IPoemMatcher, CompressedMatcher>();
builder.Services.AddScoped<IPoemMatcher, NearTraditionalMatcher>();
builder.Services.AddScoped<IPoemMatcher, TankaMatcher>();
builder.Services.AddScoped<IPoemMatcher, AmericanCinquainMatcher>();
builder.Services.AddScoped<IPoemMatcher, ReverseCinquainMatcher>();
builder.Services.AddScoped<IPoemMatcher, SedokaMatcher>();
builder.Services.AddScoped<IPoemMatcher, ButterflyCinquainMatcher>();
builder.Services.AddScoped<IPoemMatcher, MirrorCinquainMatcher>();
builder.Services.AddScoped<IPoemMatcher, ChokaMatcher>();
builder.Services.AddScoped<IPoemMatcher, IsosyllabicMatcher>();

builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();

// Email provider selection driven by configuration. "Smtp" sends real mail; all other values
// (including "Console", the default) log to the console for local development.
var emailProvider = builder.Configuration.GetValue<string>("Email:Sender:Provider") ?? "Console";
if (emailProvider == "Smtp")
{
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();
}

// Cookie-based authentication: 7-day sliding window, HttpOnly.
// In development, Secure and SameSite are relaxed to support HTTP-only local testing.
builder
    .Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.Cookie.Name = "haiku-auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
        options.Cookie.SameSite = builder.Environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

app.UseExceptional();

app.UseSerilogRequestLogging();

// Production-only error handling: custom error page, HSTS, HTTPS redirect.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Custom status code pages: 404, 500, 400, 300-level, etc.
app.UseStatusCodePagesWithReExecute("/status/{0}", createScopeForStatusCodePages: true);
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsEnvironment("Debug"))
{
    app.UseMiddleware<DebugImpersonationMiddleware>();
}

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// POST /api/auth/login — Authenticates a user by email and password.
// On success, issues the haiku-auth cookie and redirects to the home page.
// On failure, redirects back to the login page.
app.MapPost(
    "/api/auth/login",
    async (HttpContext context, IMediator mediator, ILogger<Program> logger) =>
    {
        var email = context.Request.Form["email"].FirstOrDefault() ?? string.Empty;
        var password = context.Request.Form["password"].FirstOrDefault() ?? string.Empty;

        var user = await mediator.Send(new LoginUserQuery(email, password));
        if (user == null)
        {
            logger.LogWarning("Failed login attempt for {Email}", email);
            return Results.Redirect("/login");
        }

        logger.LogInformation("User {Email} logged in successfully", email);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Results.Redirect("/");
    }
);

// POST /api/auth/register — Creates a new user account.
// On success, redirects to the login page. On failure, redirects back to the registration page.
app.MapPost(
    "/api/auth/register",
    async (HttpContext context, IMediator mediator, ILogger<Program> logger) =>
    {
        var username = context.Request.Form["username"].FirstOrDefault() ?? string.Empty;
        var email = context.Request.Form["email"].FirstOrDefault() ?? string.Empty;
        var password = context.Request.Form["password"].FirstOrDefault() ?? string.Empty;

        var user = await mediator.Send(new RegisterUserCommand(email, username, password));
        if (user == null)
        {
            logger.LogWarning("Failed registration attempt for {Email}", email);
            return Results.Redirect("/register");
        }

        logger.LogInformation("New user registered: {Username} ({Email})", username, email);
        return Results.Redirect("/login");
    }
);

// POST /api/auth/logout — Signs out the current user and redirects to the home page.
app.MapPost(
    "/api/auth/logout",
    async (HttpContext context, ILogger<Program> logger) =>
    {
        var email = context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "unknown";
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        logger.LogInformation("User {Email} logged out", email);
        return Results.Redirect("/");
    }
);

app.Run();
