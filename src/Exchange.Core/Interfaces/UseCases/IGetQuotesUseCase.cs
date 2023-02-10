namespace Exchange.Core.Interfaces.UseCases;

public interface IGetQuotesUseCase
{
    Task<CryptocurrencyQuote> Handle(string cryptoCurrencyCode, CancellationToken cancellationToken);
}