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

    public Task<string> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}