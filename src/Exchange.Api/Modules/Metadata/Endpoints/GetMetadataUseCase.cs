using Exchange.Core;
using Exchange.Core.Options;
using Exchange.Core.Ports;
using Exchange.Core.Ports.UseCases;
using Microsoft.Extensions.Options;

namespace Exchange.Api.Modules.Metadata.Endpoints;

public class GetMetadataUseCase : IGetMetadataUseCase
{
    private readonly ApiOptions _options;
    private readonly IExchangeRepositoryFactory _factory;

    public GetMetadataUseCase(IExchangeRepositoryFactory factory, IOptions<ApiOptions> options)
    {
        _factory = factory;
        _options = options.Value;
    }

    public async Task<Core.Metadata?> Handle(string cryptocurrencyCode, CancellationToken cancellationToken = default)
    {
        return await _factory.GetInstance(_options.Default).GetInfoAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken);
    }
}