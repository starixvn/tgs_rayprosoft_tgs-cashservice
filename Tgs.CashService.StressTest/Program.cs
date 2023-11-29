using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBomber.CSharp;
using Tgs.CashService.Client;

var builder = Host.CreateApplicationBuilder(args);
{
    _ = builder.Logging.ClearProviders();

    _ = builder.Services
        .AddCashServiceClient()
        .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri("http://localhost:5600"));
}

using var app = builder.Build();
await app.StartAsync().ConfigureAwait(false);
{
    var client = app.Services.GetRequiredService<ICashServiceClient>();

    var scenario = Scenario.Create(
        "單一錢包壓測",
        async context =>
        {
            var walletId = Guid.Parse("ce642d54-35f9-4aa7-96e2-eae6650c5a4d");

            _ = await client.DepositedAsync(
                walletId,
                10M,
                null,
                null,
                null).ConfigureAwait(false);

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(10, TimeSpan.FromSeconds(1D), TimeSpan.FromSeconds(30D)));

    _ = NBomberRunner
        .RegisterScenarios(scenario)
        .Run();
}
