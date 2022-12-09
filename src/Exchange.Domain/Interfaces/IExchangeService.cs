namespace Exchange.Domain.Interfaces;

public interface IExchangeService
{
    Task<IEnumerable<Metadata>?> GetInfoAsync(string[] symbols, CancellationToken cancellationToken = default);

    Task<CryptoCurrencyQuote> GetQuotesAsync(string cryptoCurrencyCode, CancellationToken cancellationToken = default);
}