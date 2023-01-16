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
            services.AddSingleton<IFeatureFlagService, FeatureFlagService>();

            services.AddFeatureManagement()
                .AddFeatureFilter<ContextualFeatureFilter>()
                .AddFeatureFilter<FeatureFilter>(); 

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
