namespace Exchange.Domain.Interfaces;

public interface IExchangeService
{
    Task<Metadata> GetInfoAsync(string currencySymbol, CancellationToken cancellationToken = default);

    Task<CryptoCurrencyQuote?> GetQuotesAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default);
}