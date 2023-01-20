using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Infrastructure.Adapters;

namespace Exchange.Infrastructure;

public sealed class ExchangeRepositoryFactory : IExchangeRepositoryFactory
{
    private readonly IEnumerable<IExchangeRepository> _repositories;

    public ExchangeRepositoryFactory(IEnumerable<IExchangeRepository> repositories)
    {
        _repositories = repositories;
    }

    public IExchangeRepository GetInstance(ApiSourceType type)
    {
        return type switch
        {
            ApiSourceType.CoinMarketCapApi => _repositories.First(x => x is CoinMarketCapRepository),
            ApiSourceType.ExchangeRateApi => _repositories.First(x => x is ExchangeRateRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
