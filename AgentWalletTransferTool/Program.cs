using AgentWalletTransferTool.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.SerialNumberGenerator;

var builder = Host.CreateApplicationBuilder(args);
{
    _ = builder.Services
        .AddDbContext<TgsContext>(dbBuilder => dbBuilder
            .UseNpgsql("name=Tgs"))
        .AddCashServiceClient()
        .ConfigureHttpClient(httpClient => httpClient.BaseAddress =
            builder.Configuration.GetValue<Uri>("Tool:CashServiceBaseUrl")!)
        .Services
        .AddSerialNumberGenerator();
}

var app = builder.Build();

await app.StartAsync().ConfigureAwait(false);

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<TgsContext>();
var cashServiceClient = scope.ServiceProvider.GetRequiredService<ICashServiceClient>();
var serialNumberGenerator = scope.ServiceProvider.GetRequiredService<ISerialNumberGenerator>();

var agentsQuery = dbContext.Agents
    .Where(agent => agent.WalletId == null);

var rows = 10;
var agents = Array.Empty<Agent>();

while ((agents = await agentsQuery
    .Take(rows)
    .ToArrayAsync()
    .ConfigureAwait(false)).Length > 0)
{
    foreach (var agent in agents)
    {
        var agentWallet = await dbContext.AgentWallets
            .FindAsync(agent.DefaultWalletId)
            .ConfigureAwait(false);

        if (agentWallet is null)
            continue;

        var walletId = await cashServiceClient.CreateNewCashAsync(
            agent.Id.ToString(),
            agentWallet.CurrencyCode).ConfigureAwait(false);

        var transactionId = await serialNumberGenerator.GenerateAsync().ConfigureAwait(false);
        var updatedOn = DateTime.UtcNow;
        var agentBalance = agentWallet.Balance;

        _ = await dbContext.AgentTransactions
            .AddAsync(new()
            {
                Id = transactionId,
                AgentId = agent.Id,
                WalletId = agentWallet.Id,
                BrandId = agent.BrandId,
                Amount = -agentBalance,
                Balance = 0M,
                Type = (short)TransactionType.AdjustCompany,
                Description = "agent wallet transfer.",
                RecordedOn = updatedOn
            })
            .ConfigureAwait(false);

        var postBalance = dbContext.UpdateAgentWalletBalanceAsync(
            agent.Id,
            agent.DefaultWalletId,
            -agentBalance,
            updatedOn);

        agent.WalletId = walletId;

        _ = await dbContext.SaveChangesAsync().ConfigureAwait(false);

        //cashServiceClient.LockReceiveAsync(walletId, agentBalance, "agent wallet transfer.", )
    }
}
