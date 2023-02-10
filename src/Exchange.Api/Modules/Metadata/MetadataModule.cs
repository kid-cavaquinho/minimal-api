using System.ComponentModel.DataAnnotations;
using Exchange.Core;
using Exchange.Core.Interfaces.UseCases;
using Exchange.Core.UseCases.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Modules.Metadata;

internal sealed class MetadataModule : IModule
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
            .CacheOutput()
            .WithTags("Metadata")
            .WithName("Metadata")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns static metadata available for a cryptocurrency, this information includes details like description, symbol and id"
            });
    }
}