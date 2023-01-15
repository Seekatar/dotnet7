using dotnet7.FeatureFlags;
using System.Globalization;


internal class FeatureFlagConfigurationSource : IConfigurationSource
{
    private readonly FeatureFlagOptions _options;

    public FeatureFlagConfigurationSource(FeatureFlagOptions options)
    {
        _options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new FeatureFlagConfigurationProvider(_options);
    }
}

internal class FeatureFlagConfigurationProvider : ConfigurationProvider // MS helper to do most of IConfigurationProvider impl
{
    private readonly FeatureFlagOptions _options;
    public const string FeatureMangementPrefix = "FeatureManagement";
    public const string FilterSuffix = "EnabledFor:0:Name";

    public FeatureFlagConfigurationProvider(FeatureFlagOptions options)
    {
        _options = options;
    }

    public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        lock (Data)
        {
            return base.GetChildKeys(earlierKeys, parentPath);
        }
    }
    
    public override bool TryGet(string key, out string? value)
    {
        lock (Data)
        {
            bool ret = base.TryGet(key, out value);
            if (_options.DumpFeatureManagement && key.StartsWith("FeatureManagement:"))
                Console.WriteLine($">>>>>> Getting {key}");
            return ret;
        }
    }

    public override void Load()
    {        
        var toggles = FeatureFlagService.GetFlagNames( _options).Result;
        lock (Data)
        {
            toggles.ForEach(t => Data[$"{FeatureMangementPrefix}:{t}:{FilterSuffix}"] = FeatureFilter.FilterName);

            // add some test data for IConfiguration and FeatureManagement
            Data["WhatTimeIsIt"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Data[$"{FeatureMangementPrefix}:PLAIN.KEYB"] = "true"; // not filtered
        }

        var timer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
        timer.Elapsed += (sender, args) =>
        {
            lock (Data)
            {
                Data["WhatTimeIsIt"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }
            OnReload(); // this tells IConfiguration to update, otherwise Data is updated, but nothing else
        };
        timer.AutoReset = true;
        timer.Enabled = true;
    }

}
