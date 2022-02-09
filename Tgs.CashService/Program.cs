using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

ThreadPool.SetMinThreads(Environment.ProcessorCount * 20, Environment.ProcessorCount);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("/config/appsettings.json", true);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
    foreach (var xmlFileName in xmlFileNames)
        c.IncludeXmlComments(xmlFileName);

    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Tgs.CashService",
            Version = "v1"
        });
});

builder.Services.AddDbRepository((sp, dbBuilder) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    _ = dbBuilder
        .UseLazyLoadingProxies()
        .UseNpgsql(config.GetConnectionString("Tgs"));
});

builder.Services.AddCashServiceCore((options, sp) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    options.RedisConnectionString = config.GetConnectionString("Redis");
});
builder.Services.AddHealthChecks();

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
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    });

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
        .ConfigQueueOptions((options, sp) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            options.Url = configuration.GetConnectionString("Nats");
        })
        .SetSessionReplySubject($"tgs.cash.worker.{Environment.MachineName}.reply"))
    .ClearAllExchanges()
    .AddNatsGlobPatternExchange("*");

var app = builder.Build();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tgs.CashService v1"));

app.MapControllers();
app.MapSwagger();
app.MapHealthChecks("/healthz");
app.MapProfileViewer();

app.Run();
