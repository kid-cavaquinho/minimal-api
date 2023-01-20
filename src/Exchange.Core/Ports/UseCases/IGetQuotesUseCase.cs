namespace Exchange.Core.Ports.UseCases;

public interface IGetQuotesUseCase
{
    Task<CryptoCurrencyQuote> Handle(string cryptoCurrencyCode, CancellationToken cancellationToken);
}