using Serilog;

namespace Exchange.API.Extensions;

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
}