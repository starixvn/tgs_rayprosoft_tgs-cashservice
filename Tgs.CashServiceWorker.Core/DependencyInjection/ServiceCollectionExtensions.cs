using System.Reflection;
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
            .AddSerialNumberGenerator()
            .AddSingleton<WalletExchanger>()
            .AddHostedService<WalletExchangeBackgroundService>();
}
