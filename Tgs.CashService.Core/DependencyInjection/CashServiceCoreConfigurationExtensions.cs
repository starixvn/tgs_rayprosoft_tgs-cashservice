using System.Reflection;
using Tgs.CashService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class CashServiceCoreConfigurationExtensions
{
    public static IServiceCollection AddCashServiceCore(this IServiceCollection services)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddSingleton<WalletExchanger>()
            .AddHostedService<WalletExchangeBackgroundService>();
}
