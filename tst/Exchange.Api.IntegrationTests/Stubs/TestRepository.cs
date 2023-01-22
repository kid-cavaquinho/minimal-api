using Exchange.Core;
using Exchange.Core.Ports;

namespace Exchange.Api.IntegrationTests.Stubs;

internal class TestRepository : IExchangeRepository
{
    public Task<Metadata?> GetMetadataAsync(CryptoCurrencySymbol currencySymbol, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CryptoCurrencyQuote?> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken = default)
    {
        var fakeQuotes = new List<Quote>
        {
            new("USD", 1),
            new("BLR", 2),
            new("EUR", 3),
            new("GBP", 5),
            new("AUD", 8),
        };
            
        return await Task.FromResult(new CryptoCurrencyQuote("TST", fakeQuotes));
    }
}