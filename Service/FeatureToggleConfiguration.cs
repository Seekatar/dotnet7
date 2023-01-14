using System.Globalization;

public static class YamlConfigurationExtensions
{
    public static IConfigurationBuilder AddFeatureToggle(this IConfigurationBuilder builder)
    {
        var source = new FeatureToggleConfigurationSource(new FeatureToggleOptions() );
        builder.Add(source);
        return builder;
    }
}

internal class FeatureToggleConfigurationSource : IConfigurationSource
{
    private readonly FeatureToggleOptions _options;

    public FeatureToggleConfigurationSource(FeatureToggleOptions options)
    {
        _options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new FeatureToggleConfigurationProvider(_options);
    }
}

internal class FeatureToggleConfigurationProvider : ConfigurationProvider // MS helper to do most of IConfigurationProvider impl
{
    private readonly FeatureToggleOptions _options;

    public FeatureToggleConfigurationProvider(FeatureToggleOptions options)
    {
        _options = options;
    }

    public override bool TryGet(string key, out string? value)
    {
        bool ret = base.TryGet(key, out value);
        if (key.StartsWith("FeatureManagement:"))
            Console.WriteLine($">>>>>> Getting {key}");
        //if ((key.StartsWith("FeatureManagement:") || key.StartsWith("WhatTimeIsIt")) && !ret)
        //    throw new Exception($"Feature {key} not found");
        return ret;
    }

    public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        if ((!parentPath?.StartsWith("Logging") ?? false) && (!parentPath?.StartsWith("Kestrel") ?? false) )
            Console.WriteLine($"####### Getting child '{parentPath}' >> {string.Join(",",earlierKeys)}");
        return base.GetChildKeys(earlierKeys, parentPath);
    }

    public override void Load()
    {
        Data["WhatTimeIsIt"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        Data["FeatureManagement:TEST.KEYC"] = "true";
        Data["FeatureManagement:TEST.KEYD:EnabledFor:0:Name"] = "FilterMe";
        
        // var toggles = FeatureToggleListener.GetToggles( _options).Result;

        // toggles.ForEach(t => Data["FeatureManagement:"+t.Name] = t.IsEnabled.ToString());

        var timer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
        timer.Elapsed += (sender, args) =>
        {
            Data["WhatTimeIsIt"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Data.TryGetValue("FeatureManagement:TEST.KEYC", out var value);
            //Data["FeatureManagement:TEST.KEYC"] = string.Equals(value, "True", StringComparison.OrdinalIgnoreCase) ? "False" : "True";
            //Console.WriteLine($"C is now {Data["FeatureManagement:TEST.KEYC"]}");
            OnReload(); // this tells IConfiguration to update, otherwise Data is updated, but nothing else
        };
        timer.AutoReset = true;
        timer.Enabled = true;
    }

}
