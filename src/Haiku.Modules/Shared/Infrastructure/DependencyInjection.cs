using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haiku.Modules.Shared.Infrastructure.Persistence;

/// <summary>
/// Registers infrastructure services — the EF Core <see cref="HaikuDbContext"/> and all repository
/// implementations — into the dependency injection container.
/// </summary>
/// <remarks>
/// <para>In the Modular Monolith architecture, repositories, the <see cref="HaikuDbContext"/>,
/// and cross-cutting infrastructure live in a single assembly alongside the module code.
/// This class scans that assembly for types implementing repository interfaces and registers
/// each as a scoped service, eliminating the need for manual registration.</para>
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the <see cref="HaikuDbContext"/> with SQL Server (retry-on-failure, 3 attempts)
    /// and auto-discovers all repository implementations via assembly scanning.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register into.</param>
    /// <param name="connectionString">The SQL Server connection string for the EF Core context.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HaikuDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3))
        );

        var assembly = typeof(HaikuDbContext).Assembly;

        var repositoryInterfaces = assembly
            .GetExportedTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .ToHashSet();

        foreach (var implType in assembly.GetExportedTypes())
        {
            if (!implType.IsClass || implType.IsAbstract)
            {
                continue;
            }

            var match = implType.GetInterfaces().FirstOrDefault(iface => repositoryInterfaces.Contains(iface));
            if (match != null)
            {
                services.AddScoped(match, implType);
            }
        }

        return services;
    }
}
