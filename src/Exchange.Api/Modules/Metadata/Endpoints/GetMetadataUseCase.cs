using Exchange.Core;
using Exchange.Core.Ports;

namespace Exchange.Api.Modules.Metadata.Endpoints;

public class GetMetadataUseCase : IGetMetadataUseCase
{
    private readonly IExchangeFactory _factory;
    
    public GetMetadataUseCase(IExchangeFactory factory)
    {
        _factory = factory;
    }

    public async Task<Core.Metadata?> Handle(string cryptocurrencyCode, CancellationToken cancellationToken = default)
    {
        return await _factory.GetInstance().GetMetadataAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken);
    }
}