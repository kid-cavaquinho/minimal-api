﻿using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Exchange.Api.IntegrationTests;

public class MetadataEndpointTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public MetadataEndpointTests()
    {
        _factory = new WebApplicationFactory<Program>();
    }
    
    [Fact]
    public async Task Metadata_Should_Return_200()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/metadata/wtf");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Metadata_Should_Return_404()
    {
        // https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#inject-mock-services
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/metadata/");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}