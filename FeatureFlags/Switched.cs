using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;


/// <inheritdoc/>
public class Switched<T> : ISwitched<T> where T : class
{
    private readonly IFeatureManager _featureManager;
    private readonly string _feature;
    private readonly T _enabled;
    private readonly T _disabled;

    public Switched(IFeatureManager featureManager, string feature, T enabled, T disabled)
    {
        _featureManager = featureManager;
        _feature = feature;
        _enabled = enabled;
        _disabled = disabled;
    }

    public async Task<T> GetAsync()
    {
        return await _featureManager.IsEnabledAsync(_feature) ? _enabled : _disabled;
    }

    public async Task<T> GetAsync<TContext>(TContext context)
    {
        return await _featureManager.IsEnabledAsync(_feature, context) ? _enabled : _disabled;
    }
}

