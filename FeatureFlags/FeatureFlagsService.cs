using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using System.Collections.Concurrent;
using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

public class FeatureFlagService : IFeatureManager
{
    private ConcurrentDictionary<string, bool> _flags = new ConcurrentDictionary<string, bool>();
        
    public FeatureFlagService()
    {
        var timer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
        timer.Elapsed += (sender, args) =>
        {
            FeatureFlagConfigurationProvider.Instance?.AddFeature("CNTXT.KEYD");
            _flags["PLAIN.KEYB"] = false;
        };
        timer.Enabled = true;
        timer.AutoReset = false;
    }

    public async IAsyncEnumerable<string> GetFeatureNamesAsync()
    {
        await Task.Delay(0);
        yield return "CNTXT.KEYB";
    }

    public Task<bool> IsEnabledAsync(string featureName)
    {
        // toggle what we got before
        if (_flags.TryGetValue(featureName, out bool value))
        {
            if (_flags.TryUpdate(featureName, !value, value))
                value = !value;
            else // TODO retry?
                throw new Exception($"Failed to update flag {featureName}");
        }
        else
        {
            value = true;
            _flags.TryAdd(featureName, true);
        }
        return Task.FromResult(value);
    }

    public Task<bool> IsEnabledAsync<TContext>(string featureName, TContext mycontext)
    {
        // just return what the context has
        return Task.FromResult((mycontext as FeatureContext)?.EnableMe ?? false);
    }

    public static Task<List<string>> GetFlagNames(FeatureFlagOptions options)
    {
        return Task.FromResult(new List<string>() { "CNTXT.KEYB" });
    }
}