using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Adapters;

public abstract class HttpService
{
    private readonly ILogger<HttpClient> _logger;
    private readonly HttpClient _httpClient;

    protected HttpService(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, string httpClientName)
    {
        _httpClient = httpClientFactory.CreateClient(httpClientName);
        _logger = loggerFactory.CreateLogger<HttpClient>();
    }

    protected async Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
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