using Exchange.Core;
using Exchange.Core.Ports;

namespace Exchange.Api.Modules.Quotes.Endpoints;

public sealed class GetQuotesUseCase : IGetQuotesUseCase
{
    private readonly IExchangeFactory _factory;

    public GetQuotesUseCase(IExchangeFactory factory)
    {
        _factory = factory;
    }

    public async Task<CryptoCurrencyQuote> Handle(string cryptocurrencyCode, CancellationToken cancellationToken)
    {
        var cryptoCurrencySymbol = new CryptoCurrencySymbol(cryptocurrencyCode);
        var quotes = await _factory.GetInstance().GetQuotesAsync(cryptoCurrencySymbol, cancellationToken);
        return quotes ?? new CryptoCurrencyQuote(cryptocurrencyCode, Enumerable.Empty<Quote>());
    }
}