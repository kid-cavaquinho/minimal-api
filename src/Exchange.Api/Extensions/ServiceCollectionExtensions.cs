using Exchange.Api.Middlewares;
using Microsoft.OpenApi.Models;

namespace Exchange.Api.Extensions;

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

    internal static void AddMiddleware(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
    }
}