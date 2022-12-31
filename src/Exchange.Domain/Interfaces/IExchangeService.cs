namespace Exchange.Domain.Interfaces;

public interface IExchangeService
{
    Task<Metadata> GetInfoAsync(string symbol, CancellationToken cancellationToken = default);

    Task<CryptoCurrencyQuote?> GetQuotesAsync(string cryptoCurrencyCode, CancellationToken cancellationToken = default);
}