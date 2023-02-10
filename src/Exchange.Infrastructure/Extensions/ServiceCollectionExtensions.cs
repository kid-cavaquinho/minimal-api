using System.Net;
using System.Net.Mime;
using Exchange.Core.Domain;
using Exchange.Core.Interfaces.Repositories;
using Exchange.Core.Options;
using Exchange.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Exchange.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IExchangeRepository>(serviceProvider => 
        {
            var type = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value.Default;
            
            return type switch
            {
                ApiSourceType.ExchangeRatesApi => serviceProvider.GetRequiredService<ExchangeRatesRepository>(),
                ApiSourceType.CoinMarketCapApi => serviceProvider.GetRequiredService<CoinMarketCapRepository>(),
                _ => serviceProvider.GetRequiredService<CoinMarketCapRepository>()
            };
        });
        
        services.AddOptions<ApiOptions>().BindConfiguration(nameof(ApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .ValidateOnStart();
        
        services.AddScoped<ExchangeRatesRepository>();
        services.AddScoped<CoinMarketCapRepository>();
        
        services.AddOptions<CoinMarketCapApiOptions>().BindConfiguration(nameof(CoinMarketCapApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        services.AddHttpClient(nameof(CoinMarketCapRepository), (sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<CoinMarketCapApiOptions>>().Value;
            httpClient.BaseAddress = options.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json); 
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        
        services.AddOptions<ExchangeRatesApiOptions>().BindConfiguration(nameof(ExchangeRatesApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        services.AddHttpClient(nameof(ExchangeRatesRepository), (sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<ExchangeRatesApiOptions>>().Value;
            httpClient.BaseAddress = options.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json); 
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.Add("apikey", options.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
    }
}