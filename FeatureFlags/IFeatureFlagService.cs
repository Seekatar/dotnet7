namespace dotnet7.FeatureFlags;

public interface IFeatureFlagService
{
    Task<bool> IsEnabled(string featureName);
    Task<bool> IsEnabled(string featureName, FeatureContext context);
}
