using System.Net.Http.Json;
using System.Text.Json;
using Exchange.Core.Ports;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapHttpService : IHttpService
{
    private readonly ILogger<CoinMarketCapHttpService> _logger;
    private readonly HttpClient _httpClient;

    public CoinMarketCapHttpService(ILogger<CoinMarketCapHttpService> logger, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
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