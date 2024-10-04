using Tgs.CashService;
using Tgs.CashService.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCashServiceRepositories(
        this IServiceCollection services)
        => services
            .AddScoped<IDbAcess, DbAccess>()
            .AddTransient<ITgsRepository, TgsRepository>(sp => ActivatorUtilities.CreateInstance<TgsRepository>(
                sp));
}
