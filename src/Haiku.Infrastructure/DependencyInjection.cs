using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haiku.Infrastructure;

/// <summary>
/// Registers infrastructure services with the dependency injection container.
/// Scans the infrastructure assembly for concrete repository implementations
/// and registers each against its domain interface as a scoped service.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the EF Core <see cref="HaikuDbContext"/> and all repository implementations.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="connectionString">The SQL Server connection string for the EF Core context.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HaikuDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3))
        );

        var infrastructureAssembly = typeof(HaikuDbContext).Assembly;
        var domainAssembly = typeof(IUserRepository).Assembly;

        var repositoryInterfaces = domainAssembly
            .GetExportedTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .ToHashSet();

        foreach (var implType in infrastructureAssembly.GetExportedTypes())
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
