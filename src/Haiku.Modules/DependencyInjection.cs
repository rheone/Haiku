using FluentValidation;
using MicroMediator;
using Microsoft.Extensions.DependencyInjection;

namespace Haiku.Modules;

/// <summary>
/// Registers application-layer services, mediators, validators, classifiers, syllable providers,
/// rhyme providers, and engines into the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Haiku application services and infrastructure with the service collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Registers MicroMediator handlers, FluentValidation validators, <see cref="IPoemClassifier"/> implementations,
    /// <see cref="ISyllableProvider"/> chain, <see cref="IRhymeProvider"/> implementations, and singleton engines
    /// (<see cref="PoemClassifierChain"/>, <see cref="SyllableEngine"/>, <see cref="RhymingEngine"/>, <see cref="PoemEngine"/>).
    /// </para>
    /// <para>
    /// Concrete service classes ending in "Service" are registered as scoped. Classifiers, syllable providers,
    /// and rhyme providers are resolved from the assembly via reflection.
    /// </para>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add registrations to.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationAssemblyReference).Assembly;

        services.AddMediator(assembly);
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();

        // IPoemClassifier — all implementations (singleton; PoemClassifierChain sorts by Priority)
        foreach (
            var type in assembly
                .GetExportedTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IPoemClassifier).IsAssignableFrom(t))
        )
        {
            services.AddSingleton(typeof(IPoemClassifier), type);
        }

        // ISyllableProvider — registered explicitly to maintain chain order:
        //   1. CustomDictionaryProvider  (user-contributed words, DB-backed)
        //   2. CmuDictionaryProvider      (CMU pronunciation dictionary, registered in Program.cs)
        //   3. HeuristicSyllableProvider   (vowel-group fallback, always last)
        services.AddSingleton<ISyllableProvider, CustomDictionaryProvider>();
        // CmuDictionaryProvider is registered as ISyllableProvider in Program.cs
        services.AddSingleton<ISyllableProvider, HeuristicSyllableProvider>();

        // IRhymeProvider — all implementations
        foreach (
            var type in assembly
                .GetExportedTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IRhymeProvider).IsAssignableFrom(t))
        )
        {
            services.AddSingleton(typeof(IRhymeProvider), type);
        }

        // Interface→implementation pairs (singleton)
        services.AddSingleton<IWordTokenizer, WordTokenizer>();

        // Interface→implementation pairs (scoped)
        services.AddScoped<IPoemInputService, PoemInputService>();

        // Concrete service classes (scoped), excludes PoemInputService (registered via IPoemInputService)
        foreach (
            var type in assembly
                .GetExportedTypes()
                .Where(t =>
                    t is { IsClass: true, IsAbstract: false, IsPublic: true }
                    && t.Name.EndsWith("Service")
                    && t != typeof(PoemInputService)
                )
        )
        {
            services.AddScoped(type);
        }

        // Concrete singleton engines and chain
        services.AddSingleton<PoemClassifierChain>();
        services.AddSingleton<SyllableEngine>();
        services.AddSingleton<RhymingEngine>();
        services.AddSingleton<PoemEngine>(sp => new PoemEngine(
            sp.GetRequiredService<PoemClassifierChain>(),
            sp.GetRequiredService<SyllableEngine>(),
            sp.GetRequiredService<CmuDictionaryProvider>(),
            sp.GetRequiredService<RhymingEngine>()
        ));

        return services;
    }
}
