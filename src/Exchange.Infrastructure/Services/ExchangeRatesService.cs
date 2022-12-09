using System.Text.Json;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure.Services;

public class ExchangeRatesService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly ExchangeRateApiOptions _options;
    private readonly ILogger<ExchangeRatesService> _logger;

    public ExchangeRatesService(IHttpClientFactory httpClientFactory,
        IOptions<ExchangeRateApiOptions> settings,
        ILogger<ExchangeRatesService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(ExchangeRatesService));
        _logger = logger;
        _options = settings.Value;
    }
    public Task<IEnumerable<Metadata>?> GetInfoAsync(string[] symbols, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CryptoCurrencyQuote> GetQuotesAsync(string currencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{_options.BaseAddress}exchangerates_data/latest?symbols={string.Join(",", (Currency[]) Enum.GetValues(typeof(Currency)))}&base={currencySymbol}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAsync(httpRequestMessage, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<ExchangeRateLiveResponse>(new MemoryStream(response), JsonSerializerOptions.Default, cancellationToken);

        if (result?.Rates is null || !result.Rates.Any())
            return new CryptoCurrencyQuote(currencySymbol, Enumerable.Empty<Quote>());

        return new CryptoCurrencyQuote(currencySymbol.ToUpperInvariant(), result.Rates.Select(r => new Quote(r.Key, r.Value)));
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