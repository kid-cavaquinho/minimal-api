﻿using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public sealed class ExchangeRatesService : HttpService, IExchangeService
{
    public ExchangeRatesService(ILogger<ExchangeRatesService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory, nameof(ExchangeRatesService))
    {
    }
    
    public Task<Metadata?> GetInfoAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"exchangerates_data/latest?symbols={string.Join(",", (Currency[]) Enum.GetValues(typeof(Currency)))}&base={cryptoCurrencySymbol}";
        var response = await SendAsync<ExchangeRateLatestQuotes>(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        if (response?.Rates is null || !response.Rates.Any())
            return new CryptoCurrencyQuote(cryptoCurrencySymbol.Value.ToUpperInvariant(), Enumerable.Empty<Quote>());

        return new CryptoCurrencyQuote(cryptoCurrencySymbol.Value.ToUpperInvariant(), response.Rates.Select(r => new Quote(r.Key, r.Value)));
    }
}