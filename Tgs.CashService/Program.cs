using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentdForward.OpenTelemetry.MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tgs.CashService;

var builder = WebApplication.CreateBuilder(args);

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
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new HeaderApiVersionReader("API-VERSION");
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
    });

builder.Services
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
    .AddSwaggerGen(c =>
    {
        var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
        foreach (var xmlFileName in xmlFileNames)
            c.IncludeXmlComments(xmlFileName);
    });

builder.Services
    .AddKeyedSingleton<IConnectionMultiplexer>(
        "Redis",
        (sp, _) => ConnectionMultiplexer.Connect(
            builder.Configuration.GetConnectionString("Redis")!))
    .AddKeyedSingleton(
        "Redis",
        (sp, _) => sp
            .GetRequiredKeyedService<IConnectionMultiplexer>("Redis")
            .GetDatabase())
    .AddCashServiceRepositories(
        (_, optionsBuilder) => optionsBuilder.UseNpgsql("name=Tgs"),
        sp => sp.GetRequiredKeyedService<IDatabase>("Redis"))
    .AddCashServiceCore(sp => sp.GetRequiredKeyedService<IDatabase>("Redis"))
    .Decorate<IWalletExchangeService, WalletExchangeServiceDecorator>();
builder.Services
    .AddHealthChecks();

var instrumentationExtenionFilters = new[] { "html", "ico", "png", "jpg" };
builder.Services
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
            .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
            .AddProfileViewExporter()
            .AddOtlpExporter())
    .WithMetrics(meterBuilder => meterBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter())
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

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
        .ConfigQueueOptions((options, _) => options.Url =
            builder.Configuration.GetConnectionString("Nats")!)
        .SetSessionReplySubject($"tgs.cash.worker.{Environment.MachineName}.reply"))
    .ClearAllExchanges()
    .AddNatsGlobPatternExchange("*");

builder.Services
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

var app = builder.Build();

app.UseSwaggerUI(c =>
{
    foreach (var desc in app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
        c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"Tgs.CashService {desc.GroupName}");
});

app.MapControllers();
app.MapSwagger();
app.MapHealthChecks("/healthz");
app.MapProfileViewer();

app.Run();
