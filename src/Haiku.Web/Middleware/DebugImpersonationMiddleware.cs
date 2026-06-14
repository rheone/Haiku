using System.Security.Claims;
using Haiku.Web.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Haiku.Web.Middleware;

public class DebugImpersonationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HaikuImpersonationOptions _options;

    public DebugImpersonationMiddleware(RequestDelegate next, IOptions<HaikuImpersonationOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

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
