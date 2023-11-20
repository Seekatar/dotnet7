using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

/// <summary>
/// Register singletons that can change depending on a feature flag
/// </summary>
static class RegisterFeatureExtensions
{
    /// <summary>
    /// Register a singleton that can change depending on a feature flag
    /// </summary>
    /// <typeparam name="T">Interface that will be injected</typeparam>
    /// <typeparam name="TEnabled">Instance when the flag is true</typeparam>
    /// <typeparam name="TDisabled">Instance when the flag is false </typeparam>
    /// <param name="services"></param>
    /// <param name="flagName">flag to evaluate</param>
    /// <returns></returns>
    public static IServiceCollection AddSingletonSwitcher<T, TEnabled, TDisabled>(this IServiceCollection services, string flagName) where T : class where TEnabled : class, T where TDisabled : class, T
    {
        services.AddSingleton<TEnabled>();
        services.AddSingleton<TDisabled>();

        services.AddSingleton<ISwitched<T>>(sp =>
        {
            var provider = sp.GetRequiredService<IFeatureManager>();
            return new Switched<T>(provider, flagName, sp.GetRequiredService<TEnabled>(), sp.GetRequiredService<TDisabled>());
        });
        return services;
    }

    /// <summary>
    /// Register a scoped instance that can change depending on a feature flag
    /// </summary>
    /// <typeparam name="T">Interface that will be injected</typeparam>
    /// <typeparam name="TEnabled">Instance when the flag is true</typeparam>
    /// <typeparam name="TDisabled">Instance when the flag is false </typeparam>
    /// <param name="services"></param>
    /// <param name="flagName">flag to evaluate</param>
    /// <returns></returns>
    public static IServiceCollection AddScopedSwitcher<T, TEnabled, TDisabled>(this IServiceCollection services, string flagName) where T : class where TEnabled : class, T where TDisabled : class, T
    {
        services.AddScoped<TEnabled>();
        services.AddScoped<TDisabled>();

        services.AddScoped<ISwitched<T>>(sp =>
        {
            var provider = sp.GetRequiredService<IFeatureManagerSnapshot>();
            return new Switched<T>(provider, flagName, sp.GetRequiredService<TEnabled>(), sp.GetRequiredService<TDisabled>());
        });
        return services;
    }
}

