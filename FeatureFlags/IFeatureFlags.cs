namespace dotnet7.FeatureFlags;

public class FeatureFlagService : IFeatureFlagService
{
    public static Task<List<string>> GetFlagNames(FeatureFlagOptions options)
    {
        return Task.FromResult(new List<string>() { "CNTXT.KEYB" });
    }

    public Task<bool> IsEnabled(string featureName, FeatureContext mycontext)
    {
        return Task.FromResult(mycontext.EnableMe);
    }
}