using FluentValidation;
using Haiku.Services.Haiku;
using Haiku.Services.Poems;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Rhyming;
using Haiku.Services.Syllables;
using Haiku.Services.Syllables.Providers;
using MicroMediator;
using Microsoft.Extensions.DependencyInjection;

namespace Haiku.Services;

public static class DependencyInjection
{
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
