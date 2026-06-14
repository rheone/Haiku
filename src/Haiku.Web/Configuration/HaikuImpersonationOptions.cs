namespace Haiku.Web.Configuration;

/// <summary>
/// Configures debug impersonation used by <see cref="Haiku.Web.Middleware.DebugImpersonationMiddleware"/>
/// to auto-sign unauthenticated requests in the Debug environment.
/// </summary>
public class HaikuImpersonationOptions
{
    /// <summary>
    /// Gets a value indicating whether debug impersonation is active.
    /// </summary>
    /// <value><c>true</c> if unauthenticated requests are automatically signed in; otherwise, <c>false</c>.</value>
    public bool Enabled { get; init; }

    /// <summary>
    /// Gets the default user ID assigned to impersonated sessions.
    /// </summary>
    /// <value>A string representation of the user's <see cref="Guid"/>.</value>
    public string DefaultUserId { get; init; } = Guid.Empty.ToString();

    /// <summary>
    /// Gets the default username assigned to impersonated sessions.
    /// </summary>
    /// <value>The username string used in claims.</value>
    public string DefaultUsername { get; init; } = "debug_user";

    /// <summary>
    /// Gets the default email address assigned to impersonated sessions.
    /// </summary>
    /// <value>The email address used in claims.</value>
    public string DefaultEmail { get; init; } = "debug@haiku.local";
}
