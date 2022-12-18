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
        // Lifetime differences
        // Scoped lifetime: Objects are the same within the entire scope of a request.
        // Transient lifetime: Objects are always different; a new instance is provided to every controller and every service.
        // Singleton lifetime: Objects are the same for every object and every request.
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
            // The HttpHeaders.TryAddWithoutValidation returns false if the key is null/empty OR it's in the list of invalid headers.
            // The Add method throws exceptions instead of returning false by calls the method ParseAndAddValue.
            // Internally it gets a parser for each key you try to add and only accepts the value if the parse is successful
            // (checking if the key you're trying to add is valid for that kind of request, for example).
            httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", coinMarketCapOptions.Key);
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
            httpClient.DefaultRequestHeaders.Add("apikey", exchangeRatesOptions.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
    }
}