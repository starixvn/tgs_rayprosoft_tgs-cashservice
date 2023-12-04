using System.Reflection;
using StackExchange.Redis;
using Tgs.CashService;

namespace Microsoft.Extensions.DependencyInjection;

public static class CashServiceCoreConfigurationExtensions
{
    public static IServiceCollection AddCashServiceCore(
        this IServiceCollection services,
        Func<IServiceProvider, IDatabase> redisDatabaseResolve)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddTransient<IWalletExchangeService>(sp => ActivatorUtilities.CreateInstance<WalletExchangeService>(
                sp,
                redisDatabaseResolve(sp)));
}
