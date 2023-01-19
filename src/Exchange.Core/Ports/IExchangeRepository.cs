namespace Exchange.Core.Ports;

public interface IExchangeRepository
{
    Task<Metadata?> GetInfoAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default);

    Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default);
}