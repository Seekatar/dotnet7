using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

[FilterAlias(FilterName)]
public class FeatureFilter : IContextualFeatureFilter<FeatureContext>
{
    private readonly IFeatureFlagService _featureFlags;
    private readonly ILogger<FeatureFilter> _logger;
    public const string FilterName = "TestFeatureFilter";

    public FeatureFilter(IFeatureFlagService featureFlags, ILogger<FeatureFilter> logger)
    {
        _featureFlags = featureFlags;
        _logger = logger;
    }

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext evaluationContext, FeatureContext context)
    {
        _logger.LogInformation("FeatureFiles.EvaluateAsync: {featureName} for {client} {market}", evaluationContext.FeatureName, context.ClientId, context.MarketId);
        return _featureFlags.IsEnabled(evaluationContext.FeatureName, context);
    }
}