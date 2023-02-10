namespace Exchange.Core.Interfaces.UseCases;

public interface IGetMetadataUseCase
{
    Task<CryptocurrencyMetadata?> Handle(string cryptoCurrencySymbol, CancellationToken cancellationToken = default);
}