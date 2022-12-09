using System.Net;
using System.Net.Mime;
using Exchange.API;
using Exchange.API.Middlewares;
using Exchange.Domain;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure;
using Exchange.Infrastructure.Options;
using Exchange.Infrastructure.Services;
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

app.MapGet("metadata", async ([FromQuery] string[] cryptocurrencyCodes, CoinMarketCapService service, CancellationToken cancellationToken) =>
    {
        var result = await service.GetInfoAsync(cryptocurrencyCodes, cancellationToken);
        return TypedResults.Ok(result);
    })
    .WithTags("Metadata")
    .WithName("Metadata")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Returns all static metadata available for one or more cryptocurrencies. This information includes details like logo, description and links to a cryptocurrency's technical documentation"
    })
    .Produces<IEnumerable<Metadata>>();

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
    .Produces((int)HttpStatusCode.OK, typeof(CryptoCurrencyQuote), MediaTypeNames.Application.Json);

await app.RunAsync(app.Lifetime.ApplicationStopped);

