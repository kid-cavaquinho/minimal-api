using System.Text.Json;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly CoinMarketCapApiOptions _options;
    private readonly ILogger<CoinMarketCapService> _logger;
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;

    public CoinMarketCapService(IHttpClientFactory httpClientFactory,
        IOptions<CoinMarketCapApiOptions> settings,
        ILogger<CoinMarketCapService> logger,
        IDistributedCache cache)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(CoinMarketCapService));
        _logger = logger;
        _options = settings.Value;

        _cache = cache;
        _distributedCacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
    }

    public async Task<IEnumerable<Metadata>?> GetInfoAsync(string[] symbols, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{_options.BaseAddress}v2/cryptocurrency/info?symbol={string.Join(",", symbols)}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await GetOrSetAsync<CoinMarketCapMetadata>(httpRequestMessage, cancellationToken);
        return response?.CryptocurrenciesMetadata?.Select(m => new Metadata(m.Value.First().Id, m.Value.First().Symbol));
    }

    public async Task<CryptoCurrencyQuote> GetQuotesAsync(string cryptoCurrencyCode, CancellationToken cancellationToken = default)
    {
        var metadata = await GetInfoAsync(new[] { cryptoCurrencyCode }, cancellationToken);

        if (metadata is null)
            return new CryptoCurrencyQuote(cryptoCurrencyCode, Enumerable.Empty<Quote>());

        var coinMarketCapCryptoCurrencyId = metadata.First().CurrencyId;
        
        var currencies = new[] 
        { 
            CoinMarketCapCurrencyId.Aud, 
            CoinMarketCapCurrencyId.Eur, 
            CoinMarketCapCurrencyId.Blr, 
            CoinMarketCapCurrencyId.Gbp, 
            CoinMarketCapCurrencyId.Usd 
        };

        var tasks = currencies.Select(currency => GetQuotePriceAsync(coinMarketCapCryptoCurrencyId, currency, cancellationToken));
        var quotePrices = await Task.WhenAll(tasks);

        return new CryptoCurrencyQuote(cryptoCurrencyCode, quotePrices.Select(s => new Quote(s.Item1, s.Item2)));
    }

    private async Task<(string, decimal?)> GetQuotePriceAsync(int currencyId, (int, string) convertId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{_options.BaseAddress}v2/cryptocurrency/quotes/latest?id={currencyId}&convert_id={convertId.Item1}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var result = await GetOrSetAsync<CoinMarketCapQuotesLatestResponse>(httpRequestMessage, cancellationToken);
        var quotePrice = result?.Data?[currencyId.ToString()]?["quote"]![convertId.Item1.ToString()]?["price"]?.GetValue<decimal>();
        // var lastUpdated = result?.Data?[currencyId.ToString()]?["quote"]![convertId.Item1.ToString()]?["last_updated"]?.GetValue<DateTimeOffset?>();
        return (convertId.Item2, quotePrice);
    }

    private async Task<T?> GetOrSetAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        var cacheKey = httpRequestMessage.RequestUri!.PathAndQuery;
        var cacheResult = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cacheResult is not null)
            return await JsonSerializer.DeserializeAsync<T>(new MemoryStream(cacheResult), JsonSerializerOptions.Default, cancellationToken);
    
        var result = await SendAsync(httpRequestMessage, cancellationToken);
        await _cache.SetAsync(cacheKey, result, _distributedCacheEntryOptions, cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(new MemoryStream(result), JsonSerializerOptions.Default, cancellationToken);
    }

    private async Task<byte[]> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (NotSupportedException notSupportedException)
        {
            _logger.LogError(notSupportedException, "Not supported exception was thrown while executing the request: {RequestUri}",
                httpRequestMessage.RequestUri);
        }
        catch (JsonException jsonException)
        {
            _logger.LogError(jsonException, "Json exception was thrown while executing the request: {RequestUri}",
                httpRequestMessage.RequestUri);
        }
        catch (HttpRequestException httpRequestException)
        {
            _logger.LogError(httpRequestException, "Http request exception was thrown while executing the request: {RequestUri}",
                httpRequestMessage.RequestUri);
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}