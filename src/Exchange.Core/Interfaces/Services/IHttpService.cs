namespace Exchange.Core.Interfaces.Services;

public interface IHttpService
{
    Task<T?> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default);
}