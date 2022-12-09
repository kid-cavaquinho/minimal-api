using System.Net;
using System.Net.Mime;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Options;
using Exchange.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Exchange.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        // Options
        services.AddOptions<ApiOptions>().BindConfiguration(nameof(ApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .ValidateOnStart();
        
        services.AddOptions<CoinMarketCapApiOptions>().BindConfiguration(nameof(CoinMarketCapApiOptions),
            options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        services.AddOptions<ExchangeRateApiOptions>().BindConfiguration(nameof(ExchangeRateApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        // Services
        services.AddScoped<CoinMarketCapService>();
        services.AddScoped<ExchangeRatesService>();
        services.AddScoped<Func<ApiSourceType, IExchangeService>>(serviceProvider => type =>
        {
            return type switch
            {
                ApiSourceType.ExchangeRates => serviceProvider.GetRequiredService<ExchangeRatesService>(),
                ApiSourceType.CoinMarketCap => serviceProvider.GetRequiredService<CoinMarketCapService>(),
                _ => serviceProvider.GetRequiredService<CoinMarketCapService>()
            };
        });
        
        // Use named client because of: 
        // - The app requires many distinct uses of HttpClient.
        // - Many HttpClient instances have different configuration.
        // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#named-clients
        services.AddHttpClient(nameof(CoinMarketCapService), (sp, httpClient) =>
        {
            var coinMarketCapOptions = sp.GetRequiredService<IOptions<CoinMarketCapApiOptions>>().Value;
            httpClient.BaseAddress = coinMarketCapOptions.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-CMC_PRO_API_KEY", coinMarketCapOptions.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        services.AddHttpClient(nameof(ExchangeRatesService), (sp, httpClient) =>
        {
            var exchangeRatesOptions = sp.GetRequiredService<IOptions<ExchangeRateApiOptions>>().Value;
            httpClient.BaseAddress = exchangeRatesOptions.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apikey", exchangeRatesOptions.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
    }
}