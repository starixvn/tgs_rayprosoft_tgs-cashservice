using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.CashServiceWorker;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceWorker(
        this IServiceCollection services)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddSingleton<WalletExchanger>()
            .AddHostedService<WalletExchangeBackgroundService>();
}
