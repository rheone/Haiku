using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MicroMediator;

/// <summary>
/// Provides extension methods for registering the MicroMediator pipeline with the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IMediator"/> service as scoped and scans the specified assembly
    /// for all command and query handler implementations, registering each as its service interface.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="assembly">The assembly to scan for handler implementations.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var handlerTypes = assembly
            .GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType
                        && (
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                            || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                            || i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                        )
                    )
                    .Select(i => (handlerType: t, serviceType: i))
            );

        foreach (var (handlerType, serviceType) in handlerTypes)
        {
            services.AddScoped(serviceType, handlerType);
        }

        return services;
    }
}
