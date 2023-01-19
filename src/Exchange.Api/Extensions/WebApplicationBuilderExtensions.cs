using Asp.Versioning.Conventions;
using Exchange.Api.Modules;
using Serilog;

namespace Exchange.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Services(services)
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext();
        });
        
        return builder;
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(1, 0)
            .ReportApiVersions()
            .Build();
        
        var modules = typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
        
        foreach (var module in modules)
        {
            module.MapEndpoints(app, apiVersionSet);
        }
    }
}