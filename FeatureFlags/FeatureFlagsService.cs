using System.Collections.Concurrent;

namespace dotnet7.FeatureFlags;

public class FeatureFlagService : IFeatureFlagService {
    internal record Flag(bool Value = true, bool IsContext = false);

    private ConcurrentDictionary<string, Flag> _flags = new(new List<KeyValuePair<string, Flag>>()  {
        new("PLAIN.KEYA",new Flag()),
        new("PLAIN.KEYB",new Flag(false)),
        new("PLAIN.KEYC",new Flag()),
        new("CNTXT.KEYA",new Flag(true, true)),
        new("CNTXT.KEYB",new Flag(false, true)),
        new("CNTXT.KEYC",new Flag(true, true))
    });

    public FeatureFlagService() {
    }

    public static Task<List<string>> LoadFlagNames(FeatureFlagOptions options) {
        return Task.FromResult(new List<string>() { "CNTXT.KEYB" });
    }

    public List<string> GetFlagNames() {
        return _flags.Keys.ToList();
    }


    public Task<bool> IsEnabled(string featureName) {
        if (_flags.TryGetValue(featureName, out Flag? value) && value is not null && !value.IsContext) {
            return Task.FromResult(value.Value);
        }
        return Task.FromResult(false);
    }

    public Task<bool> IsEnabled(string featureName, FeatureContext? context) {
        if (_flags.TryGetValue(featureName, out Flag? value) && value is not null && value.IsContext) {
            return Task.FromResult(value.Value);
        }
        return Task.FromResult(false);
    }

    public void Set(string featureName, bool value, FeatureContext? featureContext) {
        var flag = new Flag(value, featureContext is not null);
        _flags.AddOrUpdate(featureName, flag, (k, v) => flag);
    }
}