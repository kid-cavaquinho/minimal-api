using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using Asp.Versioning.Builder;
using Exchange.Api.Modules.Metadata.Endpoints;
using Exchange.Core.Options;
using Exchange.Core.Ports;
using Exchange.Core.Ports.UseCases;
using Exchange.Infrastructure; // Should not be referenced anywhere else
using Exchange.Infrastructure.Adapters;
using Exchange.Infrastructure.Services; // Should not be referenced anywhere else
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

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