﻿using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Services;

public sealed class ExchangeRatesService : HttpService, IExchangeService
{
    public ExchangeRatesService(ILogger<ExchangeRatesService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory, nameof(ExchangeRatesService))
    {
    }
    
    public Task<Metadata?> GetInfoAsync(string currencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var requestUri = $"exchangerates_data/latest?symbols={string.Join(",", (Currency[]) Enum.GetValues(typeof(Currency)))}&base={cryptoCurrencySymbol}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAsync<ExchangeRateLatestQuotes>(httpRequestMessage, cancellationToken);
        if (response?.Rates is null || !response.Rates.Any())
            return new CryptoCurrencyQuote(cryptoCurrencySymbol, Enumerable.Empty<Quote>());

        return new CryptoCurrencyQuote(cryptoCurrencySymbol.ToUpperInvariant(), response.Rates.Select(r => new Quote(r.Key, r.Value)));
    }
}