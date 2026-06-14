using System.Reflection;

namespace Haiku.Web.Configuration;

public class HaikuBuildInfo
{
    public string Version { get; init; } = "0.0.0";

    public string GitHash { get; init; } = "unknown";

    public string BuildDate { get; init; } = "unknown";

    public string Configuration { get; init; } = "Release";

    public string Environment { get; init; } = "Production";

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
