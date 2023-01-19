using System.Net.Mime;
using Asp.Versioning.Builder;
using Exchange.Api.Modules.Quotes.Endpoints;
using Exchange.Core;
using Exchange.Core.Options;
using Exchange.Core.Ports;
using Exchange.Core.Ports.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
                 [FromServices] IExchangeRepositoryFactory factory,
                 [FromServices] IOptions<ApiOptions> options,
                 CancellationToken cancellationToken) =>
             {
                 await Task.Delay(1);
                 return "hello world";
             })// TypedResults.Ok(await factory.GetInstance(options.Value.Default).GetQuotesAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken)))
        .WithTags("Quotes")
        .WithName("Quotes")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Returns the latest quotes in a submitted cryptocurrency code for USD, EUR, BRL, GBP and AUD currencies"
        })
        .Produces(StatusCodes.Status200OK, typeof(CryptoCurrencyQuote), MediaTypeNames.Application.Json);
    }
}