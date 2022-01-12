using Microsoft.EntityFrameworkCore;
using Tgs.CashService.Core;
using Tgs.CashService.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryConfigurationExtensions
    {
        public static IServiceCollection AddDbRepository(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        {
            services.AddDbContext<TgsContext>(efConfigure);
            return services
                .AddTransient<IWalletRepository, WalletRepository>();
        }
    }
}
