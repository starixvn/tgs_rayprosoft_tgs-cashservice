using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.CashService;

namespace Microsoft.Extensions.DependencyInjection;

public static class CashServiceCoreConfigurationExtensions
{
    public static IServiceCollection AddCashServiceCore(
        this IServiceCollection services,
        Action<CashOptions, IServiceProvider> configure)
        => services
            .AddOptions<CashOptions>()
            .Configure(configure)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<CashOptions>>().Value;

                return ActivatorUtilities.CreateInstance<RedisConnectionManager>(sp, options.RedisConnectionString);
            })
            .AddTransient<IWalletExchangeService>(sp =>
            {
                var connectionManager = sp.GetRequiredService<RedisConnectionManager>();

                return ActivatorUtilities.CreateInstance<WalletExchangeService>(sp, connectionManager.Database);
            });
}
