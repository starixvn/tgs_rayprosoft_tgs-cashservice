using FluentdForward.OpenTelemetry.MessagePack;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using NATS.Client.JetStream.Models;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.CashServiceWorker;
using Tgs.CashServiceWorker.MessageQueue;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("Otel:NodeIp")!;
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("Otel:ServiceName")!;
            fluentOptions.UseMessagePack(LogRecordFormatterResolver.GetResolverInstanceWithExtendInfo(
                "MachineInfo",
                () => new
                {
                    Environment.MachineName
                }));
        });
    });

builder.Services
    .AddSingleton(sp => ActivatorUtilities.CreateInstance<RedisConnectionManager>(
        sp,
        builder.Configuration.GetConnectionString("Redis")!))
    .AddDbRepository(
        (_, dbBuilder) => dbBuilder
            .UseNpgsql("name=Tgs"),
        sp => sp.GetRequiredService<RedisConnectionManager>().Database)
    .AddCashServiceWorker();

builder.Services
    .AddKeyedSingleton(
        "Nats",
        (sp, _) => new NatsConnection(NatsOpts.Default with
        {
            Url = builder.Configuration.GetConnectionString("Nats")!
        }))
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
        .ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
        .ConfigJetStream(new StreamConfig("tgs_cash_commit", ["tgs.cash.commit.batch"]) with
        {
            Storage = StreamConfigStorage.File,
            Retention = StreamConfigRetention.Workqueue,
            NumReplicas = 3
        })
        .AddProcessor<BidProcessor>("tgs.cash.bid")
        .AddSession<RequestSession>($"tgs.cash.worker.{Environment.MachineName}")
        .AddProcessor<GetBalanceProcessor>($"tgs.cash.balance.{Environment.MachineName}")
        .SetSessionReplySubject($"tgs.cash.worker.{Environment.MachineName}.reply"))
    .ClearAllExchanges()
    .AddNatsJetStreamGlobPatternExchange("tgs.cash.commit.batch")
    .AddNatsGlobPatternExchange("*");

var instrumentationExtenionFilters = new[] { "html", "ico", "png", "jpg" };
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName")))
    .WithTracing(
        tracingBuilder => tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddEnvironmentVariableDetector())
            .AddSource("Tgs.*")
            .AddSource("TGS.*")
            .AddSource("Valhalla.*")
            .AddNpgsql()
            .AddAspNetCoreInstrumentation(options => options.Filter =
                httpContext => !httpContext.Request.Path.HasValue
                    || !httpContext.Request.Path.StartsWithSegments("/swagger")
                    && !httpContext.Request.Path.StartsWithSegments("/health")
                    && !httpContext.Request.Path.StartsWithSegments("/metrics")
                    && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                    && !instrumentationExtenionFilters.Any(
                        ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
            .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
            .AddHttpClientInstrumentation()
            .AddProfileViewExporter()
            .AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector")))
    .WithMetrics(meterBuilder => meterBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddMeter("Tgs.*")
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter())
    .Services
    .AddProfileViewer()
    .AddHealthChecks()
    .Services
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

var app = builder.Build();

app.MapProfileViewer();
app.MapHealthChecks("/health");

SentrySdk.CaptureMessage($"Started result Tgs-CashServiceWorker at {DateTime.Now.ToString()}");

app.Run();
