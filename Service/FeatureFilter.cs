using Microsoft.FeatureManagement;

namespace dotnet7.Service;

public class MyFeatureContext
{
    public bool EnableMe { get; set; }
}

[FilterAlias("FilterMe")]
public class FeatureFilter : IContextualFeatureFilter<MyFeatureContext>
{
    private readonly IFeatureFlags _featureFlags;

    public FeatureFilter(IFeatureFlags featureFlags)
    {
        _featureFlags = featureFlags;
    }
    
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context, MyFeatureContext mycontext)
    {
        return _featureFlags.IsEnabled(context.FeatureName, mycontext);
    }
}