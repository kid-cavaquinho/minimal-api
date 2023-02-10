namespace Exchange.Core.Interfaces.Repositories;

public interface IExchangeRepository
{
    Task<CryptocurrencyMetadata?> GetMetadataAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default);

    Task<CryptocurrencyQuote?> GetQuotesAsync(string cryptoCurrencySymbol, string[] symbols, CancellationToken cancellationToken = default);
}