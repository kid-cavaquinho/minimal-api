using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers;

namespace Exchange.API;

public static class ServiceCollectionExtensions
{
    internal static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
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