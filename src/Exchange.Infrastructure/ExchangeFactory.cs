using Exchange.Core;
using Exchange.Core.Options;
using Exchange.Core.Ports;
using Exchange.Infrastructure.Adapters;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure;

public sealed class ExchangeFactory : IExchangeFactory
{
    private readonly ApiOptions _options;
    private readonly IEnumerable<IExchangeRepository> _repositories;

    public ExchangeFactory(IEnumerable<IExchangeRepository> repositories, IOptions<ApiOptions> options)
    {
        _repositories = repositories;
        _options = options.Value;
    }

    public IExchangeRepository GetInstance()
    {
        return _options.Default switch
        {
            ApiSourceType.CoinMarketCapApi => _repositories.First(x => x is CoinMarketCapRepository),
            ApiSourceType.ExchangeRatesApi => _repositories.First(x => x is ExchangeRatesRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(ApiSourceType), _options.Default, null)
        };
    }
}