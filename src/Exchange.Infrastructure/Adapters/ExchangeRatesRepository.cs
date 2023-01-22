using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Core.Ports.DTOs;
using Exchange.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Adapters;

public sealed class ExchangeRatesRepository : HttpService, IExchangeRepository
{
    public ExchangeRatesRepository(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory) : base(loggerFactory, httpClientFactory, nameof(ExchangeRatesRepository))
    {
    }
    
    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"exchangerates_data/latest?symbols={string.Join(",", (Currency[]) Enum.GetValues(typeof(Currency)))}&base={cryptoCurrencySymbol.Value}";
        var response = await SendAsync<ExchangeRateLatestQuotes>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        if (response?.Rates is null || !response.Rates.Any())
            return new CryptoCurrencyQuote(cryptoCurrencySymbol.Value.ToUpperInvariant(), Enumerable.Empty<Quote>());

        return new CryptoCurrencyQuote(cryptoCurrencySymbol.Value.ToUpperInvariant(), response.Rates.Select(r => new Quote(r.Key, r.Value)));
    }

    public Task<Metadata?> GetMetadataAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}