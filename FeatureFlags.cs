using Microsoft.FeatureManagement;

namespace FeatureFlags;

static class RegisterFeatureExtensions
{
    public static IServiceCollection AddSingletonFeature<T,TA,TB>(this IServiceCollection services, string flag) where T : class where TA : class, T where TB: class, T
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

public struct FeatureContext {
    public string? ClientId { get; set; }
    public int? MarketId { get; set; }
    public string? Role { get; set; }
    public long? UserId { get; set; }
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

