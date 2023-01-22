using System.ComponentModel.DataAnnotations;
using Asp.Versioning.Builder;
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

    public void MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet apiVersionSet)
    {
        endpoints.MapGet("metadata/{cryptocurrencyCode:required}", async Task<Core.Metadata?>
             ([Required] [FromRoute] string cryptocurrencyCode, 
                 [FromServices] IGetMetadataUseCase useCase,
                 CancellationToken cancellationToken) => await useCase.Handle(cryptocurrencyCode, cancellationToken))
             .WithTags("Metadata")
             .WithName("Metadata")
             .WithOpenApi(operation => new(operation)
             {
                 Summary = "Returns static metadata available for a cryptocurrency, this information includes details like description, symbol and id"
             });
    }
}