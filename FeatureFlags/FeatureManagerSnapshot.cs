using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

public interface IFeatureManagerEx : IFeatureManager
{
    void Set(string featureName, bool value);
}

public interface IFeatureManagerSnapshotEx : IFeatureManagerSnapshot, IFeatureManagerEx
{
}

public class FeatureManagerSnapshot : FeatureManager, IFeatureManagerSnapshotEx
{
    public FeatureManagerSnapshot(IFeatureFlagService featureFlagService) : base(featureFlagService)
    {

    }
}
