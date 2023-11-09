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
var transferDescription = "agent wallet transfer.";

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
            agent.BrandId,
            agent.Id.ToString(),
            agentWallet.CurrencyCode,
            CashType.Agent).ConfigureAwait(false);

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
                Description = transferDescription,
                RecordedOn = updatedOn
            })
            .ConfigureAwait(false);

        var postBalance = await dbContext.UpdateAgentWalletBalanceAsync(
            agent.Id,
            agent.DefaultWalletId,
            -agentBalance,
            updatedOn).ConfigureAwait(false);

        agent.WalletId = walletId;

        _ = await dbContext.SaveChangesAsync().ConfigureAwait(false);

        _ = await cashServiceClient.LockReceiveAsync(
            walletId,
            agentBalance,
            transferDescription,
            Guid.Empty,
            null).ConfigureAwait(false);

        _ = await cashServiceClient.CommitReceiveAsync(
            walletId,
            agentBalance,
            transferDescription,
            Guid.Empty,
            null).ConfigureAwait(false);
    }
}
