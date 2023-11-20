namespace dotnet7.FeatureFlags;

/// <summary>
/// Service for managing feature flags
/// </summary>
public interface IFeatureFlagService
{
    List<string> GetFlagNames();
    Task<bool> IsEnabled(string featureName);
    Task<bool> IsEnabled<TContext>(string featureName, TContext context);
    void Set(string featureName, bool value);
    void Set<TContext>(string featureName, bool value, TContext featureContext);
}
