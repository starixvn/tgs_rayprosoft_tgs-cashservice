using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.CashServiceCommitter;
using Tgs.CashServiceCommitter.Repositories;
using Tgs.CashServiceCommitter.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceCommitterRepositories(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbConfigure,
        Func<IServiceProvider, IDatabase> resolveRedisDatabase)
        => services
            .AddDbContext<TgsContext>(dbConfigure)
            .AddTransient<ITgsRepository, TgsRepository>()
            .AddTransient<ITransactionLogRepository>(sp => ActivatorUtilities.CreateInstance<RedisRepository>(
                sp,
                resolveRedisDatabase(sp)))
            .AddTransient<IDistributedResourceManager>(sp => ActivatorUtilities.CreateInstance<RedisRepository>(
                sp,
                resolveRedisDatabase(sp)));
}
