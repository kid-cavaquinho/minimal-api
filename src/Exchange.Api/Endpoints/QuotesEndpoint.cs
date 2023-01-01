using System.Net.Mime;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Exchange.Api.Endpoints;

public static class QuotesEndpoint
{
    public static void UseQuotesEndpoint(this WebApplication app)
    {
        app.MapGet("quotes/{cryptocurrencyCode:required}", async ([FromRoute] string cryptocurrencyCode,
                [FromServices] Func<ApiSourceType, IExchangeService> serviceResolver,
                [FromServices] IOptions<ApiOptions> options,
                CancellationToken cancellationToken) => TypedResults.Ok(await serviceResolver(options.Value.Default).GetQuotesAsync(cryptocurrencyCode, cancellationToken)))
            .WithTags("Quotes")
            .WithName("Quotes")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns the latest quotes in a submitted cryptocurrency code for USD, EUR, BRL, GBP and AUD currencies"
            })
            .Produces(StatusCodes.Status200OK, typeof(CryptoCurrencyQuote), MediaTypeNames.Application.Json);
    }
}