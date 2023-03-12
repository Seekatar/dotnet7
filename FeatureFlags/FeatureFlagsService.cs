using System.Collections.Concurrent;

namespace dotnet7.FeatureFlags;

/// <summary>
/// singleton service that maintains flag states for this demo
/// </summary>
public class FeatureFlagService : IFeatureFlagService {
    internal record Flag(bool Value = true, bool IsContext = false);

    private ConcurrentDictionary<string, Flag> _flags = new(new List<KeyValuePair<string, Flag>>()  {
        // for demo, these are flags
        new("PLAIN.KEYA", new Flag()),
        new("PLAIN.KEYB", new Flag(false)),
        new("PLAIN.KEYC", new Flag()),
        // for demo, context flags default to false if no context is provided
        new("CNTXT.KEYA", new Flag(true, true)),
        new("CNTXT.KEYB", new Flag(false, true)),
        new("CNTXT.KEYC", new Flag(true, true))
    });

    public FeatureFlagService() {
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

    public Task<bool> IsEnabled<TContext>(string featureName, TContext context) {
        if (_flags.TryGetValue(featureName, out Flag? value) && value is not null && value.IsContext) {
            return Task.FromResult(value.Value);
        }
        return Task.FromResult(false);
    }

    public void Set(string featureName, bool value) {
        var flag = new Flag(value);
        _flags.AddOrUpdate(featureName, flag, (k, v) => flag);
    }

    public void Set<TContext>(string featureName, bool value, TContext featureContext)
    {
        var flag = new Flag(value, IsContext: true);
        _flags.AddOrUpdate(featureName, flag, (k, v) => flag);
    }
}