using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NBomber.CSharp;
using Tgs.CashService.Client;
using System.IO;
using System.Text;

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
        .WithLoadSimulations(Simulation.RampingInject(1000, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60D)));

    var wallets100Scenario = Scenario.Create(
        "錢包100個壓測",
        async context =>
        {
            try
            {
                var idx = context.InvocationNumber % 100;
                var walletId = walletIds[idx];

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
            for (var i = 0; i <= 1001; i++)
                _ = await client.GetBalanceAsync(
                    walletIds[i]).ConfigureAwait(false);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(10000, TimeSpan.FromSeconds(1D), TimeSpan.FromMinutes(5D)));

    var wallets1000Scenario = Scenario.Create(
        "錢包1000個壓測",
        async context =>
        {
            try
            {
                var idx = context.InvocationNumber % 1000;
                var walletId = walletIds[idx];

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
            for (var i = 0; i <= 1000; i++)
                _ = await client.GetBalanceAsync(
                    walletIds[i]).ConfigureAwait(false);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(10000, TimeSpan.FromSeconds(1D), TimeSpan.FromMinutes(5D)));

    var wallets10000Scenario = Scenario.Create(
        "錢包10000個壓測",
        async context =>
        {
            try
            {
                var idx = context.InvocationNumber % 10000;
                var walletId = walletIds[idx];

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
            foreach (var batch in walletIds.Chunk(100))
                _ = await Task.WhenAll(batch
                    .Select(id => client.GetBalanceAsync(id).AsTask())
                    .ToArray()).ConfigureAwait(false);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(10000, TimeSpan.FromSeconds(1D), TimeSpan.FromMinutes(5D)));

    _ = NBomberRunner
        .RegisterScenarios(wallets1000Scenario)
        .Run();
}
