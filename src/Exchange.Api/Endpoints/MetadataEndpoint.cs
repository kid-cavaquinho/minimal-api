using System.ComponentModel.DataAnnotations;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Endpoints;

public static class MetadataEndpoint
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#typedresults-vs-results
    public static void UseMetadataEndpoint(this WebApplication app)
    {
        app.MapGet("metadata/{cryptocurrencyCode:required}", async Task<Results<Ok<Metadata>, BadRequest, NotFound>>
            ([Required] [FromRoute] string cryptocurrencyCode, [FromServices] IExchangeServiceFactory factory,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(cryptocurrencyCode))
                    return TypedResults.BadRequest();

                var result = await factory.GetInstance(ApiSourceType.CoinMarketCap).GetInfoAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken);

                if (result is null)
                    return TypedResults.NotFound();
                
                return TypedResults.Ok(result);
            })
            .WithTags("Metadata")
            .WithName("Metadata")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns static metadata available for a cryptocurrency, this information includes details like description, symbol and id"
            });
    }
}