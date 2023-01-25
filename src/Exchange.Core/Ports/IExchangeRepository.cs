using Exchange.Api.Modules.Metadata.Core;

namespace Exchange.Core.Ports;

public interface IExchangeRepository
{
    Task<CryptocurrencyMetadata?> GetMetadataAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default);

    Task<CryptocurrencyQuote?> GetQuotesAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default);
}