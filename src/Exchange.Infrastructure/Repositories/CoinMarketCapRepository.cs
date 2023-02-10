using Exchange.Core;
using Exchange.Core.Dtos;
using Exchange.Core.Interfaces.Repositories;
using Exchange.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Repositories;

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
    
    public async Task<CryptocurrencyQuote?> GetQuotesAsync(string cryptocurrencySymbol, string[] currencySymbols, CancellationToken cancellationToken = default)
    {
        var coinMarketCapCryptoCurrencyId = await GetCurrencyId(cryptocurrencySymbol, cancellationToken);
    
        if (coinMarketCapCryptoCurrencyId is null)
            return new CryptocurrencyQuote(cryptocurrencySymbol, Enumerable.Empty<Quote>());
    
        var tasks = new List<Task<(string currency, decimal? quote)>>(currencySymbols.Length);
        foreach (var currencySymbol in currencySymbols)
        {
            tasks.Add(GetQuotePriceAsync(coinMarketCapCryptoCurrencyId.Value, currencySymbol, cancellationToken));
        }
    
        var quotes = await Task.WhenAll(tasks);
    
        return new CryptocurrencyQuote(cryptocurrencySymbol.ToUpperInvariant(), quotes.Select(s => new Quote(s.currency, s.quote)));
    }
    
    private async Task<(string currency, decimal? quote)> GetQuotePriceAsync(int currencyId, string convert, CancellationToken cancellationToken = default)
    {
        // Todo: Measure and consider using ids for performance. 
        // https://coinmarketcap.com/api/documentation/v1/#section/Best-Practices
        var requestUri = $"v2/cryptocurrency/quotes/latest?id={currencyId}&convert={convert}";
        var result = await SendAsync<CoinMarketCapLatestQuotes>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        var quotePrice = result?.Data[currencyId.ToString()]?["quote"]![convert]?["price"]?.GetValue<decimal>();
        return (convert.ToUpperInvariant(), quotePrice);
    }
    
    private async Task<int?> GetCurrencyId(string cryptocurrencySymbol, CancellationToken cancellationToken = default)
    {
        return (await GetMetadataAsync(cryptocurrencySymbol, cancellationToken))?.Id;
    }
}