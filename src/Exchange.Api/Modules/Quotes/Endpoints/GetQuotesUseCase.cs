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

    public async Task<CryptoCurrencyQuote> Handle(string cryptocurrencyCode, CancellationToken cancellationToken)
    {
        var cryptoCurrencySymbol = new CryptoCurrencySymbol(cryptocurrencyCode);
        var quotes = await _repository.GetQuotesAsync(cryptoCurrencySymbol, cancellationToken);
        return quotes ?? new CryptoCurrencyQuote(cryptocurrencyCode, Enumerable.Empty<Quote>());
    }
}