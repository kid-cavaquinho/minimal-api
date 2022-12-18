using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public abstract class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpService> _logger;

    protected HttpService(ILogger<HttpService> logger, IHttpClientFactory httpClientFactory, string httpClientName)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(httpClientName);
    }

    internal async Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        var bytes = await SendAsync(httpRequestMessage, cancellationToken);
        
        try
        {
            return await JsonSerializer.DeserializeAsync<T>(new MemoryStream(bytes), JsonSerializerOptions.Default, cancellationToken);
        }
        catch (JsonException jsonException)
        {
            _logger.LogError(jsonException, "Json exception was thrown while trying to deserialize async");
        }

        return default;
    }

    private async Task<byte[]> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
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

        return Array.Empty<byte>();
    }
}