using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

[FilterAlias(FilterName)]
public class FeatureFilter : IFeatureFilter
{
    private readonly IFeatureFlagService _featureFlags;
    private readonly ILogger<ContextualFeatureFilter> _logger;
    public const string FilterName = "TestFeatureFilter";

    public FeatureFilter(IFeatureFlagService featureFlags, ILogger<ContextualFeatureFilter> logger)
    {
        _featureFlags = featureFlags;
        _logger = logger;
    }

    public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext evaluationContext)
    {
        _logger.LogInformation("FeatureFiles.EvaluateAsync: {featureName}", evaluationContext.FeatureName);
        return await _featureFlags.IsEnabled(evaluationContext.FeatureName);
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
        _logger.LogInformation(">>>> FeatureFiles.EvaluateAsync: {featureName} {enableMe} {ret} for {client} {market}", evaluationContext.FeatureName, context.EnableMe, ret, context.ClientId, context.MarketId);
        return ret;
    }
}