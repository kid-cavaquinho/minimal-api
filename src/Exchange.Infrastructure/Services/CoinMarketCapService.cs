using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapService : HttpService, IExchangeService
{
    public CoinMarketCapService(ILogger<CoinMarketCapService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory, nameof(CoinMarketCapService))
    {
    }

    public async Task<IEnumerable<Metadata>?> GetInfoAsync(string[] symbols, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/info?symbol={string.Join(",", symbols)}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAsync<CoinMarketCapMetadata>(httpRequestMessage, cancellationToken);
        return response?.CryptocurrenciesMetadata?.Select(m => new Metadata(m.Value.First().Id, m.Value.First().Symbol));
    }

    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(string cryptoCurrencyCode, CancellationToken cancellationToken = default)
    {
        var metadata = await GetInfoAsync(new[] { cryptoCurrencyCode }, cancellationToken);

        if (metadata is null)
            return new CryptoCurrencyQuote(cryptoCurrencyCode, Enumerable.Empty<Quote>());

        var coinMarketCapCryptoCurrencyId = metadata.First().CurrencyId;
        
        var coinMarketCapCurrencies = new[] 
        { 
            CoinMarketCapCurrencyId.Aud,
            CoinMarketCapCurrencyId.Eur, 
            CoinMarketCapCurrencyId.Blr, 
            CoinMarketCapCurrencyId.Gbp, 
            CoinMarketCapCurrencyId.Usd 
        };

        var tasks = coinMarketCapCurrencies.Select(convertCurrency => GetQuotePriceAsync(coinMarketCapCryptoCurrencyId, convertCurrency, cancellationToken));
        var quotePrices = await Task.WhenAll(tasks);

        return new CryptoCurrencyQuote(cryptoCurrencyCode, quotePrices.Select(s => new Quote(s.currency, s.quote)));
    }

    private async Task<(string currency, decimal? quote)> GetQuotePriceAsync(int currencyId, (int Id, string Name) convertCurrency, CancellationToken cancellationToken = default)
    {
        var requestUri = $"v2/cryptocurrency/quotes/latest?id={currencyId}&convert_id={convertCurrency.Id}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var result = await SendAsync<CoinMarketCapLatestQuotes>(httpRequestMessage, cancellationToken);
        var quotePrice = result?.Data?[currencyId.ToString()]?["quote"]![convertCurrency.Id.ToString()]?["price"]?.GetValue<decimal>();
        return (convertCurrency.Name, quotePrice);
    }
}