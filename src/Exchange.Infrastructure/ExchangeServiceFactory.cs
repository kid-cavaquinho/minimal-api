using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Services;

namespace Exchange.Infrastructure;

public sealed class ExchangeServiceFactory : IExchangeServiceFactory
{
    private readonly IEnumerable<IExchangeService> _exchangeServices;

    public ExchangeServiceFactory(IEnumerable<IExchangeService> exchangeServices)
    {
        _exchangeServices = exchangeServices;
    }

    public IExchangeService GetInstance(ApiSourceType type)
    {
        return type switch
        {
            ApiSourceType.CoinMarketCap => _exchangeServices.First(x => x is CoinMarketCapService),
            ApiSourceType.ExchangeRates => _exchangeServices.First(x => x is ExchangeRatesService),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
