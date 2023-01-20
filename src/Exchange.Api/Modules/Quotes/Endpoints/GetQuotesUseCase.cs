using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Core.Ports.UseCases;

namespace Exchange.Api.Modules.Quotes.Endpoints;

public sealed class GetQuotesUseCase : IGetQuotesUseCase
{
    private readonly IExchangeRepositoryFactory _factory;

    public GetQuotesUseCase(IExchangeRepositoryFactory factory)
    {
        _factory = factory;
    }

    public async Task<CryptoCurrencyQuote> Handle(string cryptocurrencyCode, CancellationToken cancellationToken)
    {
        var repository = _factory.GetInstance();
        var cryptoCurrencySymbol = new CryptoCurrencySymbol(cryptocurrencyCode);

        // Todo: Fetch metadata id's and pass conversion id's 

        var quotes = await repository.GetQuotesAsync(cryptoCurrencySymbol, cancellationToken);
        return quotes ?? new CryptoCurrencyQuote(cryptocurrencyCode, Enumerable.Empty<Quote>());
    }
}