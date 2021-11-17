using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Tgs.CashService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tgs.CashService v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger();
                endpoints.MapHealthChecks("/healthz");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddSwaggerGen(c =>
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

            services.AddDbRepository((sp, dbBuilder) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                dbBuilder.UseLazyLoadingProxies();
                dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"));
            });

            services.AddCashServiceCore();

            services
                .AddHealthChecks();
        }
    }
}
