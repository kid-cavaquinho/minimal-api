using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapService : HttpService, IExchangeService
{
    public CoinMarketCapService(ILogger<CoinMarketCapService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory, nameof(CoinMarketCapService))
    {
    }
    
    public async Task<Metadata> GetInfoAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/info?symbol={symbol}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAsync<CoinMarketCapMetadata>(httpRequestMessage, cancellationToken);
        // Todo: Handle nullables
        var cryptocurrencyMetadataCoinMarketCap = response!.CryptocurrenciesMetadata!.First().Value.First();
        return new Metadata(cryptocurrencyMetadataCoinMarketCap.Id, cryptocurrencyMetadataCoinMarketCap.Symbol);
    }
    
    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(string cryptoCurrencyCode, CancellationToken cancellationToken = default)
    {
        var coinMarketCapCryptoCurrencyId = await GetCurrencyId(cryptoCurrencyCode, cancellationToken);

        if (coinMarketCapCryptoCurrencyId is null)
            return new CryptoCurrencyQuote(cryptoCurrencyCode, Enumerable.Empty<Quote>());

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

        return new CryptoCurrencyQuote(cryptoCurrencyCode.ToUpperInvariant(), quotePrices.Select(s => new Quote(s.currency, s.quote)));
    }

    private async Task<(string currency, decimal? quote)> GetQuotePriceAsync(int currencyId, (int Id, string Name) convertCurrency, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/quotes/latest?id={currencyId}&convert_id={convertCurrency.Id}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var result = await SendAsync<CoinMarketCapLatestQuotes>(httpRequestMessage, cancellationToken);
        var quotePrice = result?.Data?[currencyId.ToString()]?["quote"]![convertCurrency.Id.ToString()]?["price"]?.GetValue<decimal>();
        return (convertCurrency.Name, quotePrice);
    }
    
    private async Task<int?> GetCurrencyId(string cryptoCurrencyCode, CancellationToken cancellationToken = default)
    {
        var response = await GetInfoAsync(cryptoCurrencyCode, cancellationToken);
        return response?.CurrencyId;
    }
}