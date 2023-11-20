using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags;

public static class FeatureFlagExtensions {
    public static IServiceCollection AddFeatureFlags(this IServiceCollection services) {
        services.AddSingleton<IFeatureFlagService, FeatureFlagService>();

        services.AddSingleton<FeatureManager>();
        services.AddSingleton<IFeatureManager>((svc) => svc.GetRequiredService<FeatureManager>());
        services.AddSingleton<IFeatureManagerEx>((svc) => svc.GetRequiredService<FeatureManager>());

        services.AddScoped<FeatureManagerSnapshot>();
        services.AddScoped<IFeatureManagerSnapshot>((svc) => svc.GetRequiredService<FeatureManagerSnapshot>());
        services.AddScoped<IFeatureManagerSnapshotEx>((svc) => svc.GetRequiredService<FeatureManagerSnapshot>());

        return services;
    }
}
