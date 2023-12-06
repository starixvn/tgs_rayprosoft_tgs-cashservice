using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tgs.CashService.Client;
using WalletSanpshotTransferTool.Entities;

var builder = Host.CreateApplicationBuilder(args);
{
    _ = builder.Services
        .AddDbContext<TgsContext>(dbBuilder => dbBuilder
            .UseNpgsql("name=Tgs"))
        .AddCashServiceClient()
        .ConfigureHttpClient(httpClient => httpClient.BaseAddress =
            builder.Configuration.GetValue<Uri>("Tool:CashServiceBaseUrl")!);
}

using var app = builder.Build();

await app.StartAsync().ConfigureAwait(false);

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<TgsContext>();
var cashServiceClient = scope.ServiceProvider.GetRequiredService<ICashServiceClient>();

var waitProcessWallets = await dbContext.Wallets
    .Where(agent => agent.SnapshotId == null)
    .ToArrayAsync()
    .ConfigureAwait(false);

foreach (var row in waitProcessWallets)
    _ = await cashServiceClient.GetBalanceAsync(row.Id).ConfigureAwait(false);
