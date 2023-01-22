using Exchange.Core;
using Exchange.Core.Ports;

namespace Exchange.Api.Modules.Metadata.Endpoints;

public class GetMetadataUseCase : IGetMetadataUseCase
{
    private readonly IExchangeRepository _repository;
    
    public GetMetadataUseCase(IExchangeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Core.Metadata?> Handle(string cryptocurrencyCode, CancellationToken cancellationToken = default)
    {
        return await _repository.GetMetadataAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken);
    }
}