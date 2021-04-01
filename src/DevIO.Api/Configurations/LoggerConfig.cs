using System;
using DevIO.Api.Extensions;
using Elmah.Io.AspNetCore;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "388dd3a277cb44c4aa128b5c899a3106";
                o.LogId = new Guid("c468b2b8-b35d-4f1a-849d-f47b60eef096");
            });

            //services.AddLogging(builder =>
            //{
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "388dd3a277cb44c4aa128b5c899a3106";
            //        o.LogId = new Guid("c468b2b8-b35d-4f1a-849d-f47b60eef096");
            //    });
            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            services.AddHealthChecks()
                //.AddElmahIoPublisher("388dd3a277cb44c4aa128b5c899a3106", new Guid("c468b2b8-b35d-4f1a-849d-f47b60eef096"), application: "DevIO Api")
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();
            app.UseHealthChecks("/api/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            //app.UseHealthChecksUI(options => { options.UIPath = "/api/hc-ui" });
            return app;
        }
    }
}