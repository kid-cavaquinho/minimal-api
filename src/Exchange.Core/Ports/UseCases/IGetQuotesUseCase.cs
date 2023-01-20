namespace Exchange.Core.Ports.UseCases;

public interface IGetQuotesUseCase
{
    Task<string> Handle(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken);
}