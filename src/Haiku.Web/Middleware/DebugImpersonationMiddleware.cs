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
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            var config = context.RequestServices.GetRequiredService<IConfiguration>();
            var impersonationSection = config.GetSection("Impersonation");
            if (impersonationSection.GetValue<bool>("Enabled"))
            {
                var userId = impersonationSection.GetValue<string>("DefaultUserId") ?? Guid.Empty.ToString();
                var username = impersonationSection.GetValue<string>("DefaultUsername") ?? "debug_user";
                var email = impersonationSection.GetValue<string>("DefaultEmail") ?? "debug@haiku.local";

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, userId),
                    new(ClaimTypes.Name, username),
                    new(ClaimTypes.Email, email),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }
        }

        await _next(context);
    }
}
