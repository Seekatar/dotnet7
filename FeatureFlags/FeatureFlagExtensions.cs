using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;

namespace dotnet7.FeatureFlags
{
    public static class FeatureFlagExtensions
    {
        public static IServiceCollection AddFeatureFlags(this IServiceCollection services, IConfigurationBuilder configuration, bool ignoreMissingFeatures = false)
        {
            configuration.AddFeatureFlag(); // add my IConfiguration provider

            services.Configure<FeatureManagementOptions>(options =>
            {
                options.IgnoreMissingFeatures = ignoreMissingFeatures;
                options.IgnoreMissingFeatureFilters = false;
            });

            services.AddFeatureManagement();
            services.RemoveAll<IFeatureManager>(); // v2 of FM doesn't do TryAddSingleton so we need to remove the default one
            services.AddSingleton<IFeatureManager, FeatureFlagService>();

            return services;
        }

        /// <summary>
        /// Add the configuration source for feature flags to plug into FeatureManagement
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddFeatureFlag(this IConfigurationBuilder builder)
        {
            // get config from temp configuration
            var tempConfig = builder.Build();
            var options = tempConfig.GetSection(FeatureFlagOptions.SectionName).Get<FeatureFlagOptions>() ?? new FeatureFlagOptions();

            var source = new FeatureFlagConfigurationSource(options);
            builder.Add(source);
            return builder;
        }

    }
}
