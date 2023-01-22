namespace Exchange.Core.Ports;

public interface IGetMetadataUseCase
{
    Task<Metadata?> Handle(string cryptocurrencyCode, CancellationToken cancellationToken = default);
}