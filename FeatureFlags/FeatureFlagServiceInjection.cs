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
    /// <typeparam name="TA">Instance when the flag is true</typeparam>
    /// <typeparam name="TB">Instance when the flag is false </typeparam>
    /// <param name="services"></param>
    /// <param name="flag">flag to evaluate</param>
    /// <returns></returns>
    public static IServiceCollection AddSingletonFeature<T, TA, TB>(this IServiceCollection services, string flag) where T : class where TA : class, T where TB : class, T
    {
        services.AddSingleton<TA>();
        services.AddSingleton<TB>();

        services.AddSingleton<IFeature<T>>(sp =>
        {
            var provider = sp.GetRequiredService<IFeatureManager>();
            return new Feature<T>(provider, flag, sp.GetRequiredService<TA>(), sp.GetRequiredService<TB>());
        });
        return services;
    }
}

interface IFeature<T> where T : class
{
    Task<T> GetFeature(FeatureContext? context = null);
}

public class Feature<T> : IFeature<T> where T : class
{
    private readonly IFeatureManager _flagProvider;
    private string _flag;
    private readonly T _featureA;
    private readonly T _featureB;

    public Feature(IFeatureManager flagProvider, string flag, T featureA, T featureB)
    {
        _flagProvider = flagProvider;
        _flag = flag;
        _featureA = featureA;
        _featureB = featureB;
    }

    public async Task<T> GetFeature(FeatureContext? context = null)
    {
        return await _flagProvider.IsEnabledAsync(_flag) ? _featureA : _featureB;
    }
}

interface IToggledFeature
{
    string GetResult();
}

public class ToggledFeatureA : IToggledFeature
{
    public string GetResult()
    {
        return "This is from A";
    }
}

public class ToggledFeatureB : IToggledFeature
{
    public string GetResult()
    {
        return "This is from B";
    }

}

