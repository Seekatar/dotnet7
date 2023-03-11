using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

[FilterAlias(FilterName)]
public class FeatureFilter : IFeatureFilter
{
    public const string FilterName = "TestFeatureFilter";

    private readonly ILogger<ContextualFeatureFilter> _logger;
    
    public FeatureFilter(ILogger<ContextualFeatureFilter> logger)
    {
        _logger = logger;
    }
    
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext evaluationContext)
    {
        var ret = false;
        _logger.LogInformation(">>>> FeatureFilter.EvaluateAsync: {featureName} {ret}", evaluationContext.FeatureName, ret );
        return Task.FromResult(ret); // don't need this filter if always return false since FM pretty much ignores it
    }
}

[FilterAlias(FilterName)]
public class ContextualFeatureFilter : IContextualFeatureFilter<FeatureContext>
{
    private readonly IFeatureFlagService _featureFlags;
    private readonly ILogger<ContextualFeatureFilter> _logger;
    public const string FilterName = "TestContextualFeatureFilter";

    public ContextualFeatureFilter(IFeatureFlagService featureFlags, ILogger<ContextualFeatureFilter> logger)
    {
        _featureFlags = featureFlags;
        _logger = logger;
    }

    public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext evaluationContext, FeatureContext context)
    {
        var ret = await _featureFlags.IsEnabled(evaluationContext.FeatureName, context);
        _logger.LogInformation(">>>> ContextualFeatureFilter.EvaluateAsync: {featureName} {enableMe} {ret} for {client} {market}", evaluationContext.FeatureName, context.EnableMe, ret, context.ClientId, context.MarketId);
        return ret;
    }
}