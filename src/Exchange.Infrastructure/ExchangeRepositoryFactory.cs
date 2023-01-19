using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Infrastructure.Adapters;

namespace Exchange.Infrastructure;

public sealed class ExchangeRepositoryFactory : IExchangeRepositoryFactory
{
    private readonly IEnumerable<IExchangeRepository> _exchangeServices;

    public ExchangeRepositoryFactory(IEnumerable<IExchangeRepository> exchangeServices)
    {
        _exchangeServices = exchangeServices;
    }

    public IExchangeRepository GetInstance(ApiSourceType type)
    {
        return type switch
        {
            ApiSourceType.CoinMarketCapApi => _exchangeServices.First(x => x is CoinMarketCapRepository),
            // ApiSourceType.ExchangeRatesApi => _exchangeServices.First(x => x is ExchangeRatesService),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
