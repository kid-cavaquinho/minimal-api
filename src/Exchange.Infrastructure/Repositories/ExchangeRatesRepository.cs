using Exchange.Core;
using Exchange.Core.Dtos;
using Exchange.Core.Interfaces.Repositories;
using Exchange.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Repositories;

public sealed class ExchangeRatesRepository : HttpService, IExchangeRepository
{
    public ExchangeRatesRepository(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory) : base(loggerFactory, httpClientFactory, nameof(ExchangeRatesRepository))
    {
    }
    
    public async Task<CryptocurrencyQuote?> GetQuotesAsync(string cryptocurrencySymbol, string[] currencySymbols, CancellationToken cancellationToken = default)
    {
        var requestUri = $"exchangerates_data/latest?symbols={string.Join(",", currencySymbols)}&base={cryptocurrencySymbol}";
        var response = await SendAsync<ExchangeRateLatestQuotes>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        if (response?.Rates is null || !response.Rates.Any())
            return new CryptocurrencyQuote(cryptocurrencySymbol.ToUpperInvariant(), Enumerable.Empty<Quote>());

        return new CryptocurrencyQuote(cryptocurrencySymbol.ToUpperInvariant(), response.Rates.Select(r => new Quote(r.Key, r.Value)));
    }

    public Task<CryptocurrencyMetadata?> GetMetadataAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}