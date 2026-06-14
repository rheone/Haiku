namespace Haiku.Infrastructure;

public class HaikuEmailOptions
{
    public bool RequireVerification { get; init; }

    public HaikuEmailSenderOptions Sender { get; init; } = new();
}

public class HaikuEmailSenderOptions
{
    public string Provider { get; init; } = "Console";
}
