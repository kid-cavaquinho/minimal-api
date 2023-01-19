using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using Asp.Versioning.Builder;
using Exchange.Api.Modules.Metadata.Endpoints;
using Exchange.Core.Options;
using Exchange.Core.Ports;
using Exchange.Core.Ports.UseCases;
using Exchange.Infrastructure; // Should not be referenced anywhere else
using Exchange.Infrastructure.Adapters; // Should not be referenced anywhere else
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Exchange.Api.Modules.Metadata;

public sealed class MetadataModule : IModule
{
    public void AddModule(IServiceCollection services)
    {
        services.AddOptions<CoinMarketCapApiOptions>().BindConfiguration(nameof(CoinMarketCapApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        services.AddHttpClient(nameof(CoinMarketCapRepository), (sp, httpClient) =>
        {
            var coinMarketCapOptions = sp.GetRequiredService<IOptions<CoinMarketCapApiOptions>>().Value;
            httpClient.BaseAddress = coinMarketCapOptions.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            // The HttpHeaders.TryAddWithoutValidation returns false if the key is null/empty OR it's in the list of invalid headers.
            // The Add method throws exceptions instead of returning false by calls the method ParseAndAddValue.
            // Internally it gets a parser for each key you try to add and only accepts the value if the parse is successful
            // (checking if the key you're trying to add is valid for that kind of request, for example).
            httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", coinMarketCapOptions.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        // Todo: Should be registered globally
        services.AddScoped<IExchangeRepositoryFactory, ExchangeRepositoryFactory>();
        services.AddScoped<IGetMetadataUseCase, GetMetadataUseCase>();
        services.AddScoped<IExchangeRepository, CoinMarketCapRepository>();
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