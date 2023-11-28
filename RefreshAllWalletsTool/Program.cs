using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;

var builder = Host.CreateApplicationBuilder(args);
{
    _ = builder.Services
        .AddKeyedSingleton<IConnectionMultiplexer>(
            "Redis",
            (sp, _) => ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("Redis")!))
        .AddMessageQueue()
        .AddNatsMessageQueue(natsConfig => natsConfig
            .ConfigQueueOptions((queueOptions, _) => queueOptions.Url =
                builder.Configuration.GetConnectionString("Nats")!))
        .AddNatsJetStreamGlobPatternExchange("tgs.cash.commit.batch");
}

using var app = builder.Build();
await app.StartAsync().ConfigureAwait(false);
{
    var connectionMultiplexer = app.Services.GetRequiredService<IConnectionMultiplexer>();
    var database = connectionMultiplexer.GetDatabase();
    var messageSender = app.Services.GetRequiredService<IMessageSender>();

    var allWaitingCommitWalletIds = connectionMultiplexer.GetServers()
        .ToAsyncEnumerable()
        .SelectMany(server => server.KeysAsync(pattern: "Tgs:Transactions:*", cursor: 0L, pageSize: 10000))
        .Select(key => ((string)key)!.Split(':').Last())
        .Select(Guid.Parse);

    await foreach (var id in allWaitingCommitWalletIds)
    {
        var request = new CommitExchangeRequest
        {
            WalletId = id
        };

        await messageSender.PublishAsync(
            "tgs.cash.commit.batch",
            request.ToByteArray()).ConfigureAwait(false);
    }
}
await app.StopAsync().ConfigureAwait(false);
