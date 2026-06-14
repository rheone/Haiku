using System.Reflection;
using System.Security.Claims;
using Haiku.Domain.Interfaces;
using Haiku.Infrastructure;
using Haiku.Infrastructure.Email;
using Haiku.Services;
using Haiku.Services.Configuration;
using Haiku.Services.Slices.Auth;
using Haiku.Services.Syllables;
using Haiku.Services.Syllables.Providers;
using Haiku.Web.Components;
using Haiku.Web.Configuration;
using Haiku.Web.Middleware;
using MicroMediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using StackExchange.Exceptional;

var builder = WebApplication.CreateBuilder(args);

// Serilog is configured from appsettings and enriches all output with request-scoped context.
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();

builder.Host.UseSerilog();

// Options pattern registration for Haiku configuration sections.
builder.Services.Configure<HaikuEmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<HaikuImpersonationOptions>(builder.Configuration.GetSection("Impersonation"));
builder.Services.Configure<HaikuThemeOptions>(builder.Configuration.GetSection("Themes"));

// Build info populated from assembly metadata at build time (Version, GitHash, etc.).
var buildInfo = HaikuBuildInfo.FromAssembly(Assembly.GetEntryAssembly(), builder.Environment.EnvironmentName);
builder.Services.AddSingleton(buildInfo);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddExceptional();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=Haiku;Trusted_Connection=True;TrustServerCertificate=True;";

// Infrastructure: EF Core DbContext + all I*Repository implementations (scoped)
builder.Services.AddInfrastructure(connectionString);

// Application layer: CQRS handlers, FluentValidation, IPoemClassifier chain,
// ISyllableProvider/IRhymeProvider chain, IWordTokenizer, IPoemInputService,
// concrete *Service classes (scoped), and engines (singleton).
builder.Services.AddApplication();

// CmuDictionaryProvider needs a file path, so its singleton factory stays here.
// Registered as both concrete type (for CmuRhymeProvider) and ISyllableProvider.
// The pre-processed CMU dictionary JSON is copied to the output directory at build time
// via the Haiku.Services.csproj content item.
builder.Services.AddSingleton<CmuDictionaryProvider>(_ => new CmuDictionaryProvider(
    Path.Combine(AppContext.BaseDirectory, "Resources", "cmudict.json")
));
builder.Services.AddSingleton<ISyllableProvider>(sp => sp.GetRequiredService<CmuDictionaryProvider>());

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
// Cookie security is relaxed in non-production environments to support HTTP-only local testing.
builder
    .Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.Cookie.Name = "haiku-auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.None;
        options.Cookie.SameSite = builder.Environment.IsProduction() ? SameSiteMode.Strict : SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Bootstrap logging: environment, version, git hash, build configuration.
app.Services.GetRequiredService<HaikuBuildInfo>().LogToSerilog();

app.UseExceptional();

app.UseSerilogRequestLogging();

// Non-development error handling: custom error page, HSTS, HTTPS redirect.
// All environments except Debug/Development run production-grade error handling.
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

// Debug impersonation middleware auto-signs unauthenticated requests using
// the Impersonation section from appsettings.Debug.json. Only active in Debug.
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
