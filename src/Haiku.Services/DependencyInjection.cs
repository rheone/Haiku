using FluentValidation;
using MicroMediator;
using Microsoft.Extensions.DependencyInjection;

namespace Haiku.Services;

/// <summary>
/// Configures application-level services including Mediator and FluentValidation.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Mediator handlers and FluentValidation validators from the application assembly.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(typeof(ApplicationAssemblyReference).Assembly);
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();
        return services;
    }
}
