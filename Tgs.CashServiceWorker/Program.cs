using Microsoft.EntityFrameworkCore;
using Tgs.CashServiceWorker;
using Tgs.CashServiceWorker.MessageQueue;

ThreadPool.SetMinThreads(Environment.ProcessorCount * 20, Environment.ProcessorCount);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("/config/appsettings.json", true);

builder.Services.AddDbRepository((sp, dbBuilder) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    _ = dbBuilder
        .UseNpgsql(config.GetConnectionString("Tgs"));
});

builder.Services
    .AddCashServiceCore((options, sp) => { })
    .AddSingleton(sp =>
    {
        var config = sp.GetService<IConfiguration>();

        return new RedisConnectionManager(config.GetConnectionString("Redis"));
    });
builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
        .ConfigQueueOptions((options, sp) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            options.Url = configuration.GetConnectionString("Nats");
        })
        .AddProcessor<BidProcessor>("tgs.cash.bid")
        .AddSession<RequestSession>($"tgs.cash.worker.{Environment.MachineName}")
        .SetSessionReplySubject($"tgs.cash.worker.{Environment.MachineName}.reply"))
    .ClearAllExchanges()
    .AddNatsGlobPatternExchange("*");
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.Run();
