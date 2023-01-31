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
        var symbols = new[] 
        {
            nameof(CurrencySymbol.Brl),
            nameof(CurrencySymbol.Aud),
            nameof(CurrencySymbol.Eur),
            nameof(CurrencySymbol.Gbp),
            nameof(CurrencySymbol.Usd), 
        };
        
        var quotes = await _repository.GetQuotesAsync(cryptocurrencySymbol, symbols, cancellationToken);
        return quotes ?? new CryptocurrencyQuote(cryptocurrencySymbol, Enumerable.Empty<Quote>());
    }
}