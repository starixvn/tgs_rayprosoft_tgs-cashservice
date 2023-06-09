using Tgs.CashQueryService.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashQueryClient(
        this IServiceCollection services,
        Action<IHttpClientBuilder> configureHttpClient)
    {
        configureHttpClient?.Invoke(services.AddHttpClient<ICashQueryClient, CashQueryClient>());

        return services;
    }
}
