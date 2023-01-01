using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapService : HttpService, IExchangeService
{
    public CoinMarketCapService(ILogger<CoinMarketCapService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory, nameof(CoinMarketCapService))
    {
    }
    
    public async Task<Metadata?> GetInfoAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/info?symbol={cryptoCurrencySymbol}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAsync<CoinMarketCapMetadata>(httpRequestMessage, cancellationToken);
        if (response is null)
            return default;

        var cryptocurrencyMetadataCoinMarketCap = response.CryptocurrenciesMetadata?.FirstOrDefault().Value.FirstOrDefault();
        
        // Todo: Consider matching the symbols to assure a valid metadata response
        
        return cryptocurrencyMetadataCoinMarketCap is null 
            ? default 
            : new Metadata(cryptocurrencyMetadataCoinMarketCap.Id, cryptocurrencyMetadataCoinMarketCap.Symbol, cryptocurrencyMetadataCoinMarketCap.Description);
    }
    
    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var coinMarketCapCryptoCurrencyId = await GetCurrencyId(cryptoCurrencySymbol, cancellationToken);

        if (coinMarketCapCryptoCurrencyId is null)
            return new CryptoCurrencyQuote(cryptoCurrencySymbol, Enumerable.Empty<Quote>());

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

        var quotePrices = await Task.WhenAll(tasks);

        return new CryptoCurrencyQuote(cryptoCurrencySymbol.ToUpperInvariant(), quotePrices.Select(s => new Quote(s.currency, s.quote)));
    }

    private async Task<(string currency, decimal? quote)> GetQuotePriceAsync(int currencyId, (int Id, string Name) convertCurrency, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/quotes/latest?id={currencyId}&convert_id={convertCurrency.Id}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var result = await SendAsync<CoinMarketCapLatestQuotes>(httpRequestMessage, cancellationToken);
        var quotePrice = result?.Data?[currencyId.ToString()]?["quote"]![convertCurrency.Id.ToString()]?["price"]?.GetValue<decimal>();
        return (convertCurrency.Name, quotePrice);
    }
    
    private async Task<int?> GetCurrencyId(string cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var response = await GetInfoAsync(cryptoCurrencySymbol, cancellationToken);
        return response?.Id;
    }
}