using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Exchange.API;
using Exchange.API.Middlewares;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure;
using Exchange.Infrastructure.Options;
using Exchange.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddInfrastructure();
builder.AddSerilog();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#typedresults-vs-results
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

app.MapGet("quotes/{cryptocurrencyCode:required}", async ([FromRoute] string cryptocurrencyCode,
        [FromServices] Func<ApiSourceType, IExchangeService> serviceResolver, 
        [FromServices] IOptions<ApiOptions> options,
        CancellationToken cancellationToken) =>
    {
        var service = serviceResolver(options.Value.Default);
        var result = await service.GetQuotesAsync(cryptocurrencyCode, cancellationToken);
        return TypedResults.Ok(result);
    })
    .WithTags("Quotes")
    .WithName("Quotes")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Returns the latest quotes in a submitted cryptocurrency code for USD, EUR, BRL, GBP and AUD"
    })
    .Produces(StatusCodes.Status200OK, typeof(CryptoCurrencyQuote), MediaTypeNames.Application.Json);

await app.RunAsync(app.Lifetime.ApplicationStopped);

// To enable integration tests with `WebApplicationFactory`
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program { }