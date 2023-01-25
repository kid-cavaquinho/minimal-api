using Exchange.Core;
using Exchange.Core.Ports;

namespace Exchange.Api.Modules.Quotes.Endpoints;

public sealed class GetQuotesUseCase : IGetQuotesUseCase
{
    private readonly IExchangeRepository _repository;

    public GetQuotesUseCase(IExchangeRepository repository)
    {
        _repository = repository;
    }

    public async Task<CryptocurrencyQuote> Handle(string cryptocurrencySymbol, CancellationToken cancellationToken)
    {
        var quotes = await _repository.GetQuotesAsync(cryptocurrencySymbol, cancellationToken);
        return quotes ?? new CryptocurrencyQuote(cryptocurrencySymbol, Enumerable.Empty<Quote>());
    }
}