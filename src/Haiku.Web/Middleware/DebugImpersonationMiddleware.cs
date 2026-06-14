using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Haiku.Web.Middleware;

/// <summary>
/// Middleware that automatically signs in a debug user for local development
/// when the Impersonation configuration is enabled. Only active in the "Debug"
/// environment. Allows developers to test authenticated pages without going
/// through the login flow.
/// </summary>
/// <remarks>
/// <para>
/// This middleware is registered conditionally in <c>Program.cs</c> for the "Debug"
/// environment only. Configuration values are read from the <c>Impersonation</c> section
/// of <c>appsettings.Debug.json</c>. Because the middleware runs after authentication
/// in the pipeline, it only impersonates requests that would otherwise be anonymous.
/// </para>
/// </remarks>
public class DebugImpersonationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugImpersonationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public DebugImpersonationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the request, signing in a debug user if impersonation is enabled
    /// and no authenticated user is present.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Only inject the debug identity when no user is already authenticated.
        // The null-forgiving null-coalesce (?? true) treats a null Identity
        // (pre-authentication state) the same as an unauthenticated one.
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            var config = context.RequestServices.GetRequiredService<IConfiguration>();
            var impersonationSection = config.GetSection("Impersonation");
            if (impersonationSection.GetValue<bool>("Enabled"))
            {
                // All three values fall back to reasonable defaults so the feature
                // works without any Impersonation configuration being present.
                var userId = impersonationSection.GetValue<string>("DefaultUserId") ?? Guid.Empty.ToString();
                var username = impersonationSection.GetValue<string>("DefaultUsername") ?? "debug_user";
                var email = impersonationSection.GetValue<string>("DefaultEmail") ?? "debug@haiku.local";

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, userId),
                    new(ClaimTypes.Name, username),
                    new(ClaimTypes.Email, email),
                };

                // Sign in with the same cookie scheme (haiku-auth) used by the
                // normal login flow so downstream middleware treats this as a
                // fully authenticated session.
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }
        }

        await _next(context);
    }
}
