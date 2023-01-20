using System.Net.Http.Json;
using System.Text.Json;
using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Core.Ports.DTOs;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Adapters;

public sealed class ExchangeRatesRepository : IExchangeRepository
{
    private readonly ILogger<ExchangeRatesRepository> _logger;
    private readonly HttpClient _httpClient;
    public ExchangeRatesRepository(ILogger<ExchangeRatesRepository> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    
    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"exchangerates_data/latest?symbols={string.Join(",", (Currency[]) Enum.GetValues(typeof(Currency)))}&base={cryptoCurrencySymbol}";
        var response = await SendAsync<ExchangeRateLatestQuotes>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        if (response?.Rates is null || !response.Rates.Any())
            return new CryptoCurrencyQuote(cryptoCurrencySymbol.Value.ToUpperInvariant(), Enumerable.Empty<Quote>());

        return new CryptoCurrencyQuote(cryptoCurrencySymbol.Value.ToUpperInvariant(), response.Rates.Select(r => new Quote(r.Key, r.Value)));
    }

    public Task<Metadata?> GetMetadataAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    private async Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException httpRequestException)
        {
            _logger.LogError(httpRequestException, "Http request exception was thrown while executing the request: {RequestUri}",
                httpRequestMessage.RequestUri);
        }
        catch (TaskCanceledException taskCanceledException)
        {
            _logger.LogError(taskCanceledException, "Task cancelled exception was thrown while executing the request: {RequestUri}",
                httpRequestMessage.RequestUri);
        }
        catch (JsonException jsonException)
        {
            _logger.LogError(jsonException, "Json exception was thrown while trying to deserialize async");
        }

        return default;
    }
}