using System.Text;
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
        .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri("http://cash-api-test.rpgp.cc"));
}

using var app = builder.Build();
await app.StartAsync().ConfigureAwait(false);
{
    var client = app.Services.GetRequiredService<ICashServiceClient>();

    var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");

    var walletIds = await File.ReadAllLinesAsync("wallet_ids.txt", Encoding.UTF8)
        .ToAsyncEnumerable()
        .SelectMany(arr => arr.ToAsyncEnumerable())
        .Select(Guid.Parse)
        .ToArrayAsync()
        .ConfigureAwait(false);

    var singleWalletScenario = Scenario
        .Create("單一錢包壓測", async context =>
        {
            try
            {
                var walletId = Guid.Parse("ce642d54-35f9-4aa7-96e2-eae6650c5a4d");

                _ = await client.DepositedAsync(
                    walletId,
                    10M,
                    null,
                    null,
                    null).ConfigureAwait(false);

                return Response.Ok();
            }
            catch (Exception ex)
            {
                return Response.Fail(500, ex.ToString());
            }
        })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.RampingConstant(100, TimeSpan.FromSeconds(60D)));

    var wallets10000Scenario = Scenario.Create(
        "錢包10000個壓測",
        async context =>
        {
            try
            {
                context.Logger.Information($"{context.TestInfo.SessionId}: {context.InvocationNumber}");
                var walletId = walletIds[context.InvocationNumber - 1];

                _ = await client.DepositedAsync(
                    walletId,
                    10M,
                    null,
                    null,
                    null).ConfigureAwait(false);

                return Response.Ok();
            }
            catch (Exception ex)
            {
                return Response.Fail(500, ex.ToString());
            }
        })
        .WithInit(async context =>
        {
            foreach (var walletId in walletIds)
                _ = await client.GetBalanceAsync(walletId).ConfigureAwait(false);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(10000, TimeSpan.FromSeconds(1D), TimeSpan.FromMinutes(5D)));

    _ = NBomberRunner
        .RegisterScenarios(wallets10000Scenario)
        .Run();
}
