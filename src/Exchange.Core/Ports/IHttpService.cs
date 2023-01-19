namespace Exchange.Core.Ports;

public interface IHttpService
{
    Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default);
}