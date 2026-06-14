using System.Security.Claims;
using Haiku.Web.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Haiku.Web.Middleware;

/// <summary>
/// Middleware that auto-signs unauthenticated requests with a debug user identity.
/// Only intended for use in the Debug environment where no real authentication flow is needed.
/// </summary>
public class DebugImpersonationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HaikuImpersonationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugImpersonationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="options">The impersonation configuration used for the debug identity.</param>
    public DebugImpersonationMiddleware(RequestDelegate next, IOptions<HaikuImpersonationOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    /// <summary>
    /// Signs in the default debug user if the request is unauthenticated and impersonation is enabled.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task that represents the execution of the next middleware.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            if (_options.Enabled)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, _options.DefaultUserId),
                    new(ClaimTypes.Name, _options.DefaultUsername),
                    new(ClaimTypes.Email, _options.DefaultEmail),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }
        }

        await _next(context);
    }
}
