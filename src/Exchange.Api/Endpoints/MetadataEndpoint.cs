﻿using System.ComponentModel.DataAnnotations;
using Exchange.Domain;
using Exchange.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Endpoints;

public static class MetadataEndpoint
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#typedresults-vs-results
    public static void UseMetadataEndpoint(this WebApplication app)
    {
        app.MapGet("metadata", async Task<Results<Ok<IEnumerable<Metadata>>, BadRequest>>
            ([Required] [FromQuery] string[] cryptocurrencyCodes, CoinMarketCapService service,
                CancellationToken cancellationToken) => !cryptocurrencyCodes.Any() 
                ? TypedResults.BadRequest() 
                : TypedResults.Ok(await service.GetInfoAsync(cryptocurrencyCodes, cancellationToken)))
            .WithTags("Metadata")
            .WithName("Metadata")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Returns all static metadata available for one or more cryptocurrencies. This information includes details like logo, description and links to technical documentation"
            });
    }
}