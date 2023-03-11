namespace dotnet7.FeatureFlags;

/// <summary>
/// Wrapper interface for interface that can change depending on a feature flag
/// </summary>
/// <typeparam name="T">Type registered with <see cref="AddScopedFeature"/> or <see cref="AddScopedSingleton"/></typeparam>
public interface IFeature<T> where T : class
{
    Task<T> GetFeature(FeatureContext? context = null);
}

