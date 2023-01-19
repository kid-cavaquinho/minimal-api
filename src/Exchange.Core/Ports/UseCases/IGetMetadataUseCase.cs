namespace Exchange.Core.Ports.UseCases;

public interface IGetMetadataUseCase
{
    Task<Metadata?> Handle(string cryptocurrencyCode, CancellationToken cancellationToken = default);
}