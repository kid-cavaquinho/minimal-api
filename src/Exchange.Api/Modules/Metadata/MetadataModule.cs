using System.ComponentModel.DataAnnotations;
using Exchange.Api.Modules.Metadata.Core;
using Exchange.Api.Modules.Metadata.Endpoints;
using Exchange.Core.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Modules.Metadata;

public sealed class MetadataModule : IModule
{
    public void AddModule(IServiceCollection services)
    {
        services.AddScoped<IGetMetadataUseCase, GetMetadataUseCase>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("metadata/{cryptocurrencySymbol:required}", async Task<CryptocurrencyMetadata?>
             ([Required] [FromRoute] string cryptocurrencySymbol, 
                 [FromServices] IGetMetadataUseCase useCase,
                 CancellationToken cancellationToken) => await useCase.Handle(cryptocurrencySymbol, cancellationToken))
             .WithTags("Metadata")
             .WithName("Metadata")
             .WithOpenApi(operation => new(operation)
             {
                 Summary = "Returns static metadata available for a cryptocurrency, this information includes details like description, symbol and id"
             });
    }
}