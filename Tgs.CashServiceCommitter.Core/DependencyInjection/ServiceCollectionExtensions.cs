using System.Reflection;
using Tgs.CashServiceCommitter;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceCommitter(this IServiceCollection services)
        => services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddHostedService<CashCommitterDispatcher>()
            .AddEventBus(configuration => configuration
                .RegisterEventSource<WalletChangedEvent>(CommitterConsts.WalletChangedEventName)
                .RegisterEventSource<WalletUpdateEvent>(CommitterConsts.WalletUpdateEventName));
}
