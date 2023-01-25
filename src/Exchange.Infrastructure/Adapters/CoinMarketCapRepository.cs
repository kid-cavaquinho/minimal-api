using Exchange.Api.Modules.Metadata.Core;
using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Core.Ports.DTOs;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Adapters;

public sealed class CoinMarketCapRepository : HttpService, IExchangeRepository
{
    public CoinMarketCapRepository(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory) : base(loggerFactory, httpClientFactory, nameof(CoinMarketCapRepository))
    {
    }

    public async Task<CryptocurrencyMetadata?> GetMetadataAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/info?symbol={cryptoCurrencySymbol}";
        var response = await SendAsync<CoinMarketCapMetadata>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        if (response is null)
            return default;

        var cryptocurrencyMetadataCoinMarketCap = response.CryptocurrenciesMetadata?.FirstOrDefault().Value.FirstOrDefault();
        
        return cryptocurrencyMetadataCoinMarketCap is null 
            ? default 
            : new CryptocurrencyMetadata(cryptocurrencyMetadataCoinMarketCap.Id, cryptocurrencyMetadataCoinMarketCap.Symbol, cryptocurrencyMetadataCoinMarketCap.Description);
    }
    
    public async Task<CryptocurrencyQuote?> GetQuotesAsync(string cryptocurrencySymbol, CancellationToken cancellationToken = default)
    {
        var coinMarketCapCryptoCurrencyId = await GetCurrencyId(cryptocurrencySymbol, cancellationToken);
    
        if (coinMarketCapCryptoCurrencyId is null)
            return new CryptocurrencyQuote(cryptocurrencySymbol, Enumerable.Empty<Quote>());
    
        var coinMarketCapCurrencies = new[] 
        { 
            CoinMarketCapCurrencyId.Aud,
            CoinMarketCapCurrencyId.Eur, 
            CoinMarketCapCurrencyId.Blr, 
            CoinMarketCapCurrencyId.Gbp, 
            CoinMarketCapCurrencyId.Usd 
        };
    
        var tasks = new List<Task<(string currency, decimal? quote)>>(coinMarketCapCurrencies.Length);
        foreach (var coinMarketCapCurrency in coinMarketCapCurrencies)
        {
            tasks.Add(GetQuotePriceAsync(coinMarketCapCryptoCurrencyId.Value, coinMarketCapCurrency, cancellationToken));
        }
    
        var quotes = await Task.WhenAll(tasks);
    
        return new CryptocurrencyQuote(cryptocurrencySymbol.ToUpperInvariant(), quotes.Select(s => new Quote(s.currency, s.quote)));
    }
    
    private async Task<(string currency, decimal? quote)> GetQuotePriceAsync(int currencyId, (int Id, string Name) convertCurrency, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/quotes/latest?id={currencyId}&convert_id={convertCurrency.Id}";
        var result = await SendAsync<CoinMarketCapLatestQuotes>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        var quotePrice = result?.Data[currencyId.ToString()]?["quote"]![convertCurrency.Id.ToString()]?["price"]?.GetValue<decimal>();
        return (convertCurrency.Name, quotePrice);
    }
    
    private async Task<int?> GetCurrencyId(string cryptocurrencySymbol, CancellationToken cancellationToken = default)
    {
        return (await GetMetadataAsync(cryptocurrencySymbol, cancellationToken))?.Id;
    }
}