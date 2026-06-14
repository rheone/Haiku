namespace Haiku.Infrastructure;

#pragma warning disable SA1402 // File may only contain a single type — HaikuEmailSenderOptions is tightly coupled

/// <summary>
/// Configures email delivery settings for the Haiku application.
/// </summary>
public class HaikuEmailOptions
{
    /// <summary>
    /// Gets a value indicating whether email verification is required for new account registration.
    /// </summary>
    /// <value><c>true</c> if email verification must be completed before the account is active; otherwise, <c>false</c>.</value>
    public bool RequireVerification { get; init; }

    /// <summary>
    /// Gets the email sender configuration options.
    /// </summary>
    /// <value>The sender configuration, including provider selection.</value>
    public HaikuEmailSenderOptions Sender { get; init; } = new();
}

/// <summary>
/// Configures the email sender provider for the Haiku application.
/// </summary>
public class HaikuEmailSenderOptions
{
    /// <summary>
    /// Gets a value indicating the email provider name.
    /// </summary>
    /// <value>The provider identifier: "Console", "Smtp", or "Fake".</value>
    public string Provider { get; init; } = "Console";
}

#pragma warning restore SA1402
