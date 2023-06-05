using Microsoft.EntityFrameworkCore;
using Tgs.CashQuery.Repositories;
using Tgs.CashQuery.Repositories.Entities;
using Tgs.CashQueryService;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashQueryRepositories(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbConfigure)
        => services
            .AddDbContext<TgsContext>(dbConfigure)
            .AddScoped<ITgsRepository, TgsRepository>();
}
