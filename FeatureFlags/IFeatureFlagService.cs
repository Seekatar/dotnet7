namespace dotnet7.FeatureFlags;

public interface IFeatureFlagService
{
    List<string> GetFlagNames();
    Task<bool> IsEnabled(string featureName);
    Task<bool> IsEnabled(string featureName, FeatureContext? context);
    void Set(string featureName, bool value, FeatureContext? featureContext);
}
