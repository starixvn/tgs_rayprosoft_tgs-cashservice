using System.Text.Json.Serialization;
using FluentdForward.OpenTelemetry.MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

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
    .AddCashQuery()
    .AddCashQueryRepositories(dbBuiler => dbBuiler
        .UseNpgsql("name=Tgs"));

builder.Services
    .AddRouting(routeOptions => routeOptions.LowercaseUrls = true)
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    });

builder.Services
    .AddSwaggerGen(swaggerGenOptions =>
    {
        swaggerGenOptions.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "Cash Query Service",
                Version = "v1"
            });

        swaggerGenOptions.ApplyWebApiToolkitSupport();

        var xmlFileNames = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
        foreach (var xmlFileName in xmlFileNames)
            swaggerGenOptions.IncludeXmlComments(xmlFileName);
    });

var instrumentationExtenionFilters = new[] { "html", "ico", "png", "jpg" };
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName")))
    .WithTracing(
        tracingBuilder => tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddEnvironmentVariableDetector())
            .AddSource("Tgs.*")
            .AddSource("Valhalla.*")
            .AddAspNetCoreInstrumentation(options => options.Filter =
                httpContext => !httpContext.Request.Path.HasValue
                    || !httpContext.Request.Path.StartsWithSegments("/swagger")
                    && !httpContext.Request.Path.StartsWithSegments("/healthz")
                    && !httpContext.Request.Path.StartsWithSegments("/metrics")
                    && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                    && !instrumentationExtenionFilters.Any(
                        ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
            .AddProfileViewExporter()
            .AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector")))
    .WithMetrics(meterBuilder => meterBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter())
    .Services
    .AddProfileViewer()
    .AddHealthChecks();

var app = builder.Build();

app.MapControllers();
app.UseSwaggerUI();
app.MapSwagger();
app.MapHealthChecks("/");
app.MapHealthChecks("/healthz");

SentrySdk.CaptureMessage($"Started result Tgs-CashQueryService at {DateTime.Now.ToString()}");

app.Run();
