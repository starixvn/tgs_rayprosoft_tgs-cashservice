using Microsoft.EntityFrameworkCore;
using Tgs.CashService.Repositories;
using Tgs.CashServiceWorker;
using TGS.GameProviders.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class RepositoryConfigurationExtensions
{
    public static IServiceCollection AddDbRepository(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        => services
            .AddDbContext<TgsContext>(efConfigure)
            .AddTransient<IWalletRepository, WalletRepository>()
            .AddTransient<IDbScriptExecutor, DbScriptExecutor>();
}
