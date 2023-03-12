using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

public interface IFeatureFlag : IFeatureManager {
    void Set(string featureName, bool value);
}

public interface IFeatureFlagSnapshot : IFeatureManagerSnapshot, IFeatureFlag {
}


public class FeatureManagerSnapshot : FeatureManager, IFeatureFlagSnapshot {

    public FeatureManagerSnapshot(IFeatureFlagService featureFlagService) : base(featureFlagService)
    {
        
    }
}
