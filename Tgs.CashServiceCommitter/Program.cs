using FluentdForward.OpenTelemetry.MessagePack;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using Npgsql;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using Tgs.CashServiceCommitter.MessageQueue;

var builder = WebApplication.CreateBuilder(args);
{
    _ = builder.Logging
        .AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
            _ = options.AddFluentdForwardExporter(fluentOptions =>
            {
                var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
                fluentOptions.Host = nodeIp;
                fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
                fluentOptions.UseMessagePack(LogRecordFormatterResolver.GetResolverInstanceWithExtendInfo(
                    "MachineInfo",
                    () => new
                    {
                        Environment.MachineName
                    }));
            })
            .AddOtlpExporter();
        });

    _ = builder.Services
        .AddKeyedSingleton<IConnectionMultiplexer>(
            "Redis",
            (sp, connectionKey) => ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString((string)connectionKey)!))
        .AddKeyedSingleton(
            "Redis",
            (sp, connectionKey) => sp
                .GetRequiredKeyedService<IConnectionMultiplexer>(connectionKey)
                .GetDatabase())
        .AddCashServiceCommitter()
        .AddCashServiceCommitterRepositories(
            dbBuilder => dbBuilder
                .UseNpgsql("name=Tgs"),
            sp => sp.GetRequiredKeyedService<IDatabase>("Redis"));

    _ = builder.Services
        .AddKeyedSingleton(
        "Nats",
        (sp, _) => new NatsConnection(NatsOpts.Default with
        {
            Url = builder.Configuration.GetConnectionString("Nats")!
        }))
        .AddMessageQueue()
        .AddNatsMessageQueue(natsConfig => natsConfig
            .ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
            .ConfigJetStream(new StreamConfig("tgs_cash_commit", ["tgs.cash.commit.batch"]) with
            {
                Storage = StreamConfigStorage.File,
                Retention = StreamConfigRetention.Workqueue
            })
            .AddJetStreamHandler<CommitExchangeDispatchHandler>(
                "tgs.cash.commit.batch",
                "tgs_cash_commit",
                new ConsumerConfig("tgs_cash_commit_batch") with
                {
                    MaxAckPending = 1
                })
            .AddProcessor<BidProcessor>("tgs.cash.commit.bid")
            .AddReplyHandler<BookkeepHandler>($"tgs.cash.commit.batch.{Environment.MachineName}"))
        .AddNatsGlobPatternExchange("*");

    var instrumentationExtenionFilters = new[] { "html", "ico", "png", "jpg" };
    _ = builder.Services
        .AddOpenTelemetry()
        .WithTracing(
            tracingBuilder => tracingBuilder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddEnvironmentVariableDetector())
                .AddSource("Tgs.*")
                .AddSource("TGS.*")
                .AddSource("Valhalla.*")
                .AddNpgsql()
                .AddAspNetCoreInstrumentation()
                .AddProfileViewExporter()
                .AddOtlpExporter())
        .WithMetrics(meterBuilder => meterBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddEnvironmentVariableDetector())
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddOtlpExporter())
        .Services
        .AddHealthChecks()
        .Services
        .AddProfileViewer()
        .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
            httpContext => !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/healthz")
                && !httpContext.Request.Path.StartsWithSegments("/metrics")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !instrumentationExtenionFilters.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}")));
}

var app = builder.Build();

app.MapHealthChecks("/healthz");
app.MapProfileViewer();

app.Run();
