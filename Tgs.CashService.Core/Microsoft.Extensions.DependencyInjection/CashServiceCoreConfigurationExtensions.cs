using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CashServiceCoreConfigurationExtensions
    {
        public static IServiceCollection AddCashServiceCore(this IServiceCollection services)
            => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);
    }
}
