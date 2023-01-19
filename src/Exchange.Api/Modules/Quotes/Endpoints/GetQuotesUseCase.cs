using Exchange.Core;
using Exchange.Core.Ports.UseCases;

namespace Exchange.Api.Modules.Quotes.Endpoints;

public sealed class GetQuotesUseCase : IGetQuotesUseCase
{
    public Task<string> GetQuotesAsync(CryptoCurrencySymbol cryptoCurrencySymbol, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}