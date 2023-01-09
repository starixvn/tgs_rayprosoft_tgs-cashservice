using FluentdForward.OpenTelemetry.MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.CashServiceWorker.MessageQueue;

var builder = WebApplication.CreateBuilder(args);

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
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
            fluentOptions.UseMessagePack(LogRecordFormatterResolver.GetResolverInstanceWithExtendInfo(
                "MachineInfo",
                () => new
                {
                    Environment.MachineName
                }));
        });
    });

builder.Services
    .AddDbRepository((sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        _ = dbBuilder
            .UseNpgsql("name=Tgs");
    })
    .AddCashServiceWorker((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.RedisConnectionString = config.GetConnectionString("Redis")!;
    });

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
        .ConfigQueueOptions((options, sp) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            options.Url = configuration.GetConnectionString("Nats")!;
        })
        .AddProcessor<BidProcessor>("tgs.cash.bid")
        .AddSession<RequestSession>($"tgs.cash.worker.{Environment.MachineName}")
        .AddProcessor<GetBalanceProcessor>($"tgs.cash.balance.{Environment.MachineName}")
        .SetSessionReplySubject($"tgs.cash.worker.{Environment.MachineName}.reply"))
    .ClearAllExchanges()
    .AddNatsGlobPatternExchange("*");
builder.Services
    .AddHealthChecks();

builder.Services
    .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
        sp => new ConfigureNamedOptions<OtlpExporterOptions>(
            Microsoft.Extensions.Options.Options.DefaultName,
            exporterOptions =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var nodeIp = config.GetValue<string>("NODE_IP");

                if (!string.IsNullOrWhiteSpace(nodeIp))
                    exporterOptions.Endpoint = new Uri($"http://{nodeIp}:4317");

                config.GetSection("Opentelemetry:Tracing").Bind(exporterOptions);
            }))
    .AddOpenTelemetryTracing(
        tracingBuilder => tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddEnvironmentVariableDetector())
            .AddSource("Tgs.*")
            .AddSource("TGS.*")
            .AddNpgsql()
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
            .AddProfileViewExporter()
            .AddOtlpExporter())
    .AddProfileViewer()
    .Configure<AspNetCoreInstrumentationOptions>(options =>
    {
        options.Filter = httpContext =>
        {
            return !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/healthz")
                && !httpContext.Request.Path.StartsWithSegments("/metrics")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    });

builder.Services
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

var app = builder.Build();

app.MapProfileViewer();
app.MapHealthChecks("/healthz");

app.Run();
