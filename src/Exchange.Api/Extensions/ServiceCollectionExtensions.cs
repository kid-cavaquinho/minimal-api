using Exchange.Api.Middlewares;
using Exchange.Api.Modules;
using Microsoft.OpenApi.Models;

namespace Exchange.Api.Extensions;

public static class ServiceCollectionExtensions
{
    internal static void AddModules(this IServiceCollection services)
    {
        var modules = typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
        
        foreach (var module in modules)
        {
            module.AddModule(services);
        }
    }

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