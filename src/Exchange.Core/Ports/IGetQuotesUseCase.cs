namespace Exchange.Core.Ports;

public interface IGetQuotesUseCase
{
    Task<CryptoCurrencyQuote> Handle(string cryptoCurrencyCode, CancellationToken cancellationToken);
}