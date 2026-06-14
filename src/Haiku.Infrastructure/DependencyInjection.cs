using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haiku.Infrastructure;

public static class DependencyInjection
{
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
