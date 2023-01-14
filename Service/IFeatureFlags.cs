namespace dotnet7.Service;

public interface IFeatureFlags
{
    Task<bool> IsEnabled(string featureName, MyFeatureContext mycontext);
}

public class FeatureFlagService : IFeatureFlags
{
    public Task<bool> IsEnabled(string featureName, MyFeatureContext mycontext)
    {
        return Task.FromResult(mycontext.EnableMe);
    }
}