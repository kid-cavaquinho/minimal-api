using System.Net.Mime;
using Exchange.Core;
using Exchange.Core.Interfaces.UseCases;
using Exchange.Core.UseCases.Quotes;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Modules.Quotes;

internal sealed class QuotesModule : IModule
{
    public void AddModule(IServiceCollection services)
    {
        services.AddScoped<IGetQuotesUseCase, GetQuotesUseCase>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("quotes/{cryptocurrencySymbol:required}", async ([FromRoute] string cryptocurrencySymbol,
                    [FromServices] IGetQuotesUseCase useCase,
                    CancellationToken cancellationToken)
                => await useCase.Handle(cryptocurrencySymbol, cancellationToken))
            .CacheOutput()
            .WithTags("Quotes")
            .WithName("Quotes")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns the latest quotes in a submitted cryptocurrency code for USD, EUR, BRL, GBP and AUD currencies"
            })
            .Produces(StatusCodes.Status200OK, typeof(CryptocurrencyQuote), MediaTypeNames.Application.Json);
    }
}