using System.Text.Json.Serialization;
using FluentdForward.OpenTelemetry.MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
    .WithTracing(
        tracingBuilder => tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddEnvironmentVariableDetector())
            .AddSource("Tgs.*")
            .AddSource("Valhalla.*")
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
    .AddProfileViewer()
    .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !instrumentationExtenionFilters.Any(
                ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
    .AddHealthChecks();

var app = builder.Build();

app.MapControllers();
app.UseSwaggerUI();
app.MapSwagger();
app.MapHealthChecks("/healthz");

app.Run();
