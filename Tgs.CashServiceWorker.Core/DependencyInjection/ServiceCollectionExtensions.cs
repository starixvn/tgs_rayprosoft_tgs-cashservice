using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.CashService.Core;
using Tgs.CashService.Worker;
using Tgs.CashServiceWorker;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceWorker(
        this IServiceCollection services,
        Action<WorkerOptions, IServiceProvider> configure)
        => services
            .AddOptions<WorkerOptions>()
            .Configure(configure)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddSingleton<WalletExchanger>()
            .AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<WorkerOptions>>().Value;

                return ActivatorUtilities.CreateInstance<RedisConnectionManager>(sp, options.RedisConnectionString);
            })
            .AddHostedService<WalletExchangeBackgroundService>();
}
