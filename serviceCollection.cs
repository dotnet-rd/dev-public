
// https://github.com/RehanSaeed/HttpClientSample/blob/698af3530a03709b19f55ab3fb6babdccc62f4f6/HttpClientSample/Framework/ServiceCollectionExtensions.cs
using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace test.Framework
{
    public static class ServiceCollectionExtensions
    {
        private const string BackendProvidersConfigurationSectionName = "BackendProviders";

        public static IServiceCollection AddPolicies(
            this IServiceCollection services)
        {
            var policyRegistry = services.AddPolicyRegistry();

            policyRegistry.Add(
                PolicyName.HttpTimeout,
                Policy.TimeoutAsync(30, TimeoutStrategy.Pessimistic)
                      .AsAsyncPolicy<HttpResponseMessage>());

            policyRegistry.Add(
                PolicyName.HttpRetry,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            policyRegistry.Add(
                PolicyName.HttpCircuitBreaker,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .AdvancedCircuitBreakerAsync(
                        failureThreshold: 0.5,
                        samplingDuration: TimeSpan.FromSeconds(10),
                        minimumThroughput: 8,
                        durationOfBreak: TimeSpan.FromSeconds(30)
                    ));

            return services;
        }

        public static IServiceCollection AddProviderHttpClient(
            this IServiceCollection services,
            string providerName,
            Action<IHttpClientBuilder> customConfiguration = null)
        {
            return AddProviderHttpClientCommon(services, services.AddHttpClient(providerName), null, customConfiguration);
        }

        public static IServiceCollection AddProviderHttpClient(
            this IServiceCollection services,
            IConfiguration configuration,
            string providerName,
            Action<IHttpClientBuilder> customConfiguration = null)
        {
            var section = configuration.GetSection(BackendProvidersConfigurationSectionName).GetSection(providerName);

            return AddProviderHttpClientCommon(services, services.AddHttpClient(providerName), section.Get<HttpClientOptions>(), customConfiguration);
        }

        public static IServiceCollection AddProviderHttpClient(
            this IServiceCollection services,
            IConfiguration configuration,
            string providerName,
            string[] policies,
            Action<IHttpClientBuilder> customConfiguration = null)
        {
            var section = configuration.GetSection(BackendProvidersConfigurationSectionName).GetSection(providerName);

            return AddProviderHttpClientCommon(services, services.AddHttpClient(providerName), section.Get<HttpClientOptions>(), customConfiguration, policies);
        }

        public static IServiceCollection AddProviderHttpClient<TClient, TImplementation>(
            this IServiceCollection services,
            Action<IHttpClientBuilder> customConfiguration = null)
            where TClient : class
            where TImplementation : class, TClient
        {
            return AddProviderHttpClientCommon(services, services.AddHttpClient<TClient, TImplementation>(), null, customConfiguration);
        }

        public static IServiceCollection AddProviderHttpClient<TClient, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string providerName,
            Action<IHttpClientBuilder> customConfiguration = null)
            where TClient : class
            where TClientOptions : HttpClientOptions, new()
        {
            var section = configuration.GetSection($"{BackendProvidersConfigurationSectionName}:{providerName}");

            if (typeof(TClientOptions) != typeof(HttpClientOptions))
            {
                services.TryAddSingleton<TClientOptions>(section.Get<TClientOptions>());
            }

            return AddProviderHttpClientCommon(services, services.AddHttpClient<TClient>(providerName), section.Get<HttpClientOptions>(), customConfiguration);
        }

        public static IServiceCollection AddProviderHttpClient<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string providerName,
            Action<IHttpClientBuilder> customConfiguration = null)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new()
        {
            var section = configuration.GetSection($"{BackendProvidersConfigurationSectionName}:{providerName}");

            if (typeof(TClientOptions) != typeof(HttpClientOptions))
            {
                services.TryAddSingleton<TClientOptions>(section.Get<TClientOptions>());
            }

            return AddProviderHttpClientCommon(services, services.AddHttpClient<TClient, TImplementation>(providerName), section.Get<HttpClientOptions>(), customConfiguration);
        }

        private static IServiceCollection AddProviderHttpClientCommon(
            IServiceCollection services,
            IHttpClientBuilder builder,
            HttpClientOptions config,
            Action<IHttpClientBuilder> customConfiguration,
            string[] policies)
        {
            services.TryAddTransient<UserAgentDelegatingHandler>();

            builder
                .ConfigureHttpClient((sp, options) =>
                {
                    options.BaseAddress = config?.BaseAddress;
                })
                .AddHttpMessageHandler<UserAgentDelegatingHandler>();

            foreach (var policy in policies)
            {
                builder.AddPolicyHandlerFromRegistry(policy);
            }

            customConfiguration?.Invoke(builder);

            return builder.Services;
        }

        private static IServiceCollection AddProviderHttpClientCommon(
            IServiceCollection services,
            IHttpClientBuilder builder,
            HttpClientOptions config,
            Action<IHttpClientBuilder> customConfiguration)
        {
            return AddProviderHttpClientCommon(services, builder, config, customConfiguration,
                new[] {
                    PolicyName.HttpTimeout,
                    PolicyName.HttpRetry,
                    PolicyName.HttpCircuitBreaker,
                });
        }

        public static IServiceCollection AddConfiguration<TConfiguration>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName,
            bool optional = false)
            where TConfiguration : class, new()
        {
            var config = configuration.GetSection<TConfiguration>(sectionName, optional);

            services.TryAddSingleton(config);

            return services;
        }

        public static IServiceCollection AddSystemClock(this IServiceCollection services)
        {
            services.TryAddSingleton<IClock, SystemClock>();

            return services;
        }
    }
}
