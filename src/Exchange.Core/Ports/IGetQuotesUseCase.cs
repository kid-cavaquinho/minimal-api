namespace Exchange.Core.Ports;

public interface IGetQuotesUseCase
{
    Task<CryptocurrencyQuote> Handle(string cryptoCurrencyCode, CancellationToken cancellationToken);
}