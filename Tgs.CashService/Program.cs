using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("/config/appsettings.json", true);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
    foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);

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

builder.Services.AddCashServiceCore();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tgs.CashService v1"));

app.MapControllers();
app.MapSwagger();
app.MapHealthChecks("/healthz");

app.Run();
