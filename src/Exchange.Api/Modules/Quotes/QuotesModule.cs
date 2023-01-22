using System.Net.Mime;
using Asp.Versioning.Builder;
using Exchange.Api.Modules.Quotes.Endpoints;
using Exchange.Core;
using Exchange.Core.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Modules.Quotes;

internal sealed class QuotesModule : IModule
{
    public void AddModule(IServiceCollection services)
    {
        services.AddScoped<IGetQuotesUseCase, GetQuotesUseCase>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app, ApiVersionSet apiVersionSet)
    {
         app.MapGet("quotes/{cryptocurrencyCode:required}", async ([FromRoute] string cryptocurrencyCode,
                 [FromServices] IGetQuotesUseCase useCase,
                 CancellationToken cancellationToken) 
                 => await useCase.Handle(cryptocurrencyCode, cancellationToken))
        .WithTags("Quotes")
        .WithName("Quotes")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Returns the latest quotes in a submitted cryptocurrency code for USD, EUR, BRL, GBP and AUD currencies"
        })
        .Produces(StatusCodes.Status200OK, typeof(CryptoCurrencyQuote), MediaTypeNames.Application.Json);
    }
}