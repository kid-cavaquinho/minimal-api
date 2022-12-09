using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers;

namespace Exchange.API;

public static class ServiceCollectionExtensions
{
    internal static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        var logger = new LoggerConfiguration()
            .Enrich.With(new ThreadIdEnricher())
            .WriteTo.Console()
            .CreateLogger();
        builder.Logging.AddSerilog(logger);
    }

    internal static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setup =>
        {
            // setup.OperationFilter<ApiVersionOperationFilter>();
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Description = "Minimal APIs are a simplified approach for building fast HTTP APIs with ASP.NET Core. You can build fully functioning REST endpoints with minimal code and configuration.",
                Title = "Exchange API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "João A.",
                    Email = "joao.antao@protonmail.com",
                    Url = new Uri("https://antao.github.io")
                }
            });
        });
    }
}