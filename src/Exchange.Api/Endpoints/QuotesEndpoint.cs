// using System.Net.Mime;
// using Exchange.Core;
// using Exchange.Domain.Interfaces;
// using Exchange.Infrastructure.Options;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Options;
//
// namespace Exchange.Api.Endpoints;
//
// public static class QuotesEndpoint
// {
//     public static void UseQuotesEndpoint(this WebApplication app)
//     {
//         app.MapGet("quotes/{cryptocurrencyCode:required}", async ([FromRoute] string cryptocurrencyCode,
//                 [FromServices] IExchangeServiceFactory factory,
//                 [FromServices] IOptions<ApiOptions> options,
//                 CancellationToken cancellationToken) => TypedResults.Ok(await factory.GetInstance(options.Value.Default).GetQuotesAsync(new CryptoCurrencySymbol(cryptocurrencyCode), cancellationToken)))
//             .WithTags("Quotes")
//             .WithName("Quotes")
//             .WithOpenApi(operation => new(operation)
//             {
//                 Summary = "Returns the latest quotes in a submitted cryptocurrency code for USD, EUR, BRL, GBP and AUD currencies"
//             })
//             .Produces(StatusCodes.Status200OK, typeof(CryptoCurrencyQuote), MediaTypeNames.Application.Json);
//     }
// }