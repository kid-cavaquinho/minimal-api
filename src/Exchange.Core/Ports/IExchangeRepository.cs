namespace Exchange.Core.Ports;

public interface IExchangeRepository
{
    Task<Metadata?> GetMetadataAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default);

    Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default);
}