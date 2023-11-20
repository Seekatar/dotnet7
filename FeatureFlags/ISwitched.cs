namespace dotnet7.FeatureFlags;

/// <summary>
/// Wrapper interface for class that can change depending on a feature flag
/// </summary>
/// <typeparam name="T">Type registered with <see cref="AddScopedFeature"/> or <see cref="AddScopedSingleton"/></typeparam>
public interface ISwitched<T> where T : class
{
    Task<T> GetAsync();
    Task<T> GetAsync<TContext>(TContext context);
}

