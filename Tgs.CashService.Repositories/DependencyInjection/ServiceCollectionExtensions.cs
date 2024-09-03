using Microsoft.EntityFrameworkCore;
using Tgs.CashService;
using Tgs.CashService.Repositories;
using Tgs.CashService.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        => services
            .AddDbContext<TgsContext>(efConfigure)
            .AddSerialNumberGenerator()
            .AddScoped<IDbAcess, DbAccess>()
            .AddTransient<ITgsRepository, TgsRepository>(sp => ActivatorUtilities.CreateInstance<TgsRepository>(
                sp,
                sp.GetRequiredService<TgsContext>()));
}
