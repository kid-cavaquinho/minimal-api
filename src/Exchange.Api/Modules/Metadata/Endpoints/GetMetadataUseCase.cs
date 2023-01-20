using Exchange.Core;
using Exchange.Core.Ports;
using Exchange.Core.Ports.UseCases;

namespace Exchange.Api.Modules.Metadata.Endpoints;

public class GetMetadataUseCase : IGetMetadataUseCase
{
    private readonly IExchangeRepositoryFactory _factory;

    public GetMetadataUseCase(IExchangeRepositoryFactory factory)
    {
        _factory = factory;
    }

    public async Task<Core.Metadata?> Handle(string cryptocurrencyCode, CancellationToken cancellationToken = default)
    {
        return await _factory.GetInstance().GetMetadataAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken);
    }
}