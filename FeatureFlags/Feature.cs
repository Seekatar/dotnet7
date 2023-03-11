using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

public class Feature<T> : IFeature<T> where T : class
{
    private readonly IFeatureManager _flagProvider;
    private readonly string _flag;
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

