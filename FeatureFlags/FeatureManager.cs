using Microsoft.FeatureManagement;
using System.Collections.Concurrent;

namespace dotnet7.FeatureFlags;


public class FeatureManager : IFeatureManager, IFeatureFlag {
    private readonly IFeatureFlagService _featureFlagService;

    public FeatureManager(IFeatureFlagService featureFlagService) {
        _featureFlagService = featureFlagService;
    }

    public async IAsyncEnumerable<string> GetFeatureNamesAsync() {

        foreach (var i in _featureFlagService.GetFlagNames()) {
            await Task.Delay(0);
            yield return i;
        }
    }

    public Task<bool> IsEnabledAsync(string featureName) {
        return _featureFlagService.IsEnabled(featureName);
    }

    public Task<bool> IsEnabledAsync<TContext>(string featureName, TContext context) {
        return _featureFlagService.IsEnabled(featureName, context as FeatureContext);
    }

    public void Set<TContext>(string featureName, bool value, TContext context ) {
        _featureFlagService.Set(featureName, value, context as FeatureContext);
    }

    public void Set(string featureName, bool value) {
        _featureFlagService.Set(featureName, value, null);
    }
}

