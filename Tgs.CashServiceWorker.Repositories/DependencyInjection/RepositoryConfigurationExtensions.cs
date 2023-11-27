using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.CashService.Repositories;
using Tgs.CashServiceWorker;
using TGS.GameProviders.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class RepositoryConfigurationExtensions
{
    public static IServiceCollection AddDbRepository(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> efConfigure,
        Func<IServiceProvider, IDatabase> redisResolve)
        => services
            .AddDbContext<TgsContext>(efConfigure)
            .AddTransient<IWalletRepository>(sp => new WalletRepository(
                sp.GetRequiredService<TgsContext>(),
                redisResolve(sp)));
}
