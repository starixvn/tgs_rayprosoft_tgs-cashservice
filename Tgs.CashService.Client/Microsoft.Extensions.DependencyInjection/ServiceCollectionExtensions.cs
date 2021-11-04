using System;
using Microsoft.Extensions.Options;
using Tgs.CashService.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCashServiceClient(
            this IServiceCollection services,
            Action<CashServiceClientOptions, IServiceProvider> configure)
            =>
            services
            .AddOptions<CashServiceClientOptions>()
            .Configure(configure)
            .Services
            .AddHttpClient<CashServiceClinet, CashServiceClinet>(
                (httpClient, sp) => ActivatorUtilities.CreateInstance<CashServiceClinet>(
                    sp,
                    httpClient,
                    sp.GetRequiredService<IOptions<CashServiceClientOptions>>().Value.JsonSerializerOptions!))
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<CashServiceClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services
            .AddTransient<ICashServiceClinet>(sp => sp.GetRequiredService<CashServiceClinet>());
    }
}
