namespace Haiku.Web.Configuration;

public class HaikuImpersonationOptions
{
    public bool Enabled { get; init; }

    public string DefaultUserId { get; init; } = Guid.Empty.ToString();

    public string DefaultUsername { get; init; } = "debug_user";

    public string DefaultEmail { get; init; } = "debug@haiku.local";
}
