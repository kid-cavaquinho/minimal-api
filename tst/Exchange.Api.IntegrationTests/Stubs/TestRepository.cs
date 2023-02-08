using Exchange.Core;
using Exchange.Core.Ports;

namespace Exchange.Api.IntegrationTests.Stubs;

internal class TestRepository : IExchangeRepository
{
    public Task<CryptocurrencyMetadata?> GetMetadataAsync(string cryptocurrencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CryptocurrencyQuote?> GetQuotesAsync(string cryptocurrencySymbol, string[] currencySymbols, CancellationToken cancellationToken = default)
    {
        var fakeQuotes = new List<Quote>
        {
            new("USD", 1),
            new("BLR", 2),
            new("EUR", 3),
            new("GBP", 5),
            new("AUD", 8),
        };
            
        return await Task.FromResult(new CryptocurrencyQuote("TST", fakeQuotes));
    }
}