using System.Reflection;

namespace Haiku.Web.Configuration;

/// <summary>
/// Captures build-time metadata about the running Haiku application instance.
/// Populated from assembly attributes at startup and logged for operational visibility.
/// </summary>
public class HaikuBuildInfo
{
    /// <summary>
    /// Gets the application version.
    /// </summary>
    /// <value>The semantic version string from the assembly version, or "0.0.0" if unavailable.</value>
    public string Version { get; init; } = "0.0.0";

    /// <summary>
    /// Gets the Git commit hash of the source tree at build time.
    /// </summary>
    /// <value>The full SHA hash, or "unknown" if not embedded at build time.</value>
    public string GitHash { get; init; } = "unknown";

    /// <summary>
    /// Gets the date and time the build was created.
    /// </summary>
    /// <value>The build date string, or "unknown" if not embedded at build time.</value>
    public string BuildDate { get; init; } = "unknown";

    /// <summary>
    /// Gets the build configuration (Debug or Release).
    /// </summary>
    /// <value>The build configuration name, or "Release" if unavailable.</value>
    public string Configuration { get; init; } = "Release";

    /// <summary>
    /// Gets the runtime environment name (e.g., Production, Development, Debug).
    /// </summary>
    /// <value>The environment name supplied at startup.</value>
    public string Environment { get; init; } = "Production";

    /// <summary>
    /// Logs all build metadata to Serilog at Information level for startup diagnostics.
    /// </summary>
    public void LogToSerilog()
    {
        Serilog.Log.Information(
            "Haiku starting — Environment: {Environment}, Version: {Version}, GitHash: {GitHash}, BuildDate: {BuildDate}, Configuration: {Configuration}",
            Environment,
            Version,
            GitHash,
            BuildDate,
            Configuration
        );
    }

    /// <summary>
    /// Creates a <see cref="HaikuBuildInfo"/> from assembly metadata and the runtime environment name.
    /// </summary>
    /// <param name="assembly">The entry assembly with embedded metadata attributes.</param>
    /// <param name="environmentName">The runtime environment name (e.g., Production, Development).</param>
    /// <returns>A new <see cref="HaikuBuildInfo"/> populated from the assembly attributes.</returns>
    public static HaikuBuildInfo FromAssembly(Assembly? assembly, string environmentName)
    {
        var metadata =
            assembly
                ?.GetCustomAttributes<AssemblyMetadataAttribute>()
                .ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase)
            ?? [];

        return new HaikuBuildInfo
        {
            Version = assembly?.GetName().Version?.ToString() ?? "0.0.0",
            GitHash = metadata.GetValueOrDefault("GitHash", "unknown"),
            BuildDate = metadata.GetValueOrDefault("BuildDate", "unknown"),
            Configuration = metadata.GetValueOrDefault("Configuration", "Release"),
            Environment = environmentName,
        };
    }
}
