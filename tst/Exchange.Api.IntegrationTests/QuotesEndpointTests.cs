using System.Net;
using System.Net.Http.Json;
using Exchange.Api.IntegrationTests.Stubs;
using Exchange.Core;
using Exchange.Core.Domain;
using Exchange.Core.Interfaces;
using Exchange.Core.Interfaces.Repositories;
using Exchange.Core.Ports;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Exchange.Api.IntegrationTests;

public class QuotesEndpointTests
{
    [Fact]
    public async Task Quotes_Should_Return()
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(_ => new Func<ApiSourceType, IExchangeRepository>(_ => new TestRepository())));
            });
        });

        using var client = factory.CreateClient();
        var response = await client.GetAsync("/quotes/tst");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CryptocurrencyQuote>();
        result.Should().NotBeNull();
        result!.CryptocurrencySymbol.Should().Be("TST");
        result!.Quotes.Should().HaveCount(5);
    }
}