using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.CashService;
using Tgs.CashService.Repositories;
using Tgs.CashService.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> efConfigure,
        Func<IServiceProvider, IDatabase> redisDatabaseResolve)
        => services
            .AddDbContext<TgsContext>(efConfigure)
            .AddScoped<ITgsRepository>(sp => ActivatorUtilities.CreateInstance<TgsRepository>(
                sp,
                sp.GetRequiredService<TgsContext>(),
                redisDatabaseResolve(sp)));
}
