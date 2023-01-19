namespace Exchange.Core.Ports.UseCases;

public interface IGetQuotesUseCase
{
    Task<string> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken);
}