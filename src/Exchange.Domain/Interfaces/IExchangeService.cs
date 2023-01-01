namespace Exchange.Domain.Interfaces;

public interface IExchangeService
{
    Task<Metadata?> GetInfoAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default);

    Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default);
}