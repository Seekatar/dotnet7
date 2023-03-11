using System.Collections.Concurrent;

namespace dotnet7.FeatureFlags;

public class FeatureFlagService : IFeatureFlagService {
    private readonly ConcurrentDictionary<string, bool> _flags = new();

    public FeatureFlagService() {
    }

    public static Task<List<string>> GetFlagNames(FeatureFlagOptions options) {
        return Task.FromResult(new List<string>() { "CNTXT.KEYB" });
    }

    public Task<bool> IsEnabled(string featureName) {
        // toggle what we got before
        if (_flags.TryGetValue(featureName, out bool value)) {
            if (_flags.TryUpdate(featureName, !value, value))
                value = !value;
            else // TODO retry?
                throw new Exception($"Failed to update flag {featureName}");
        }
        else {
            value = true;
            _flags.TryAdd(featureName, true);
        }
        return Task.FromResult(value);
    }

    public Task<bool> IsEnabled(string featureName, FeatureContext context) {
        // just return what the context has
        return Task.FromResult(context.EnableMe);
    }
}