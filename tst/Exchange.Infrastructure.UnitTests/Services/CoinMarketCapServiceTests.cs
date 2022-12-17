using System.Net;
using Exchange.Infrastructure.Options;
using Exchange.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Exchange.Infrastructure.UnitTests.Services;

public class CoinMarketCapServiceTests
{
    private const string CryptoCurrencyCode = "TST";
    private readonly IOptions<CoinMarketCapApiOptions> _options;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public CoinMarketCapServiceTests()
    {
        _options = Microsoft.Extensions.Options.Options.Create(new CoinMarketCapApiOptions
        {
            Key = "a-key", 
            BaseAddress = new Uri("http://localhost/")
        });
        
        _httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    }

    [Fact]
    public async Task GetInfoAsync_Should_Return_Empty_Array_Quotes_When_Deserialization_Fails()
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("failure")
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        httpClient.BaseAddress = _options.Value.BaseAddress;
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(CoinMarketCapService))).Returns(httpClient).Verifiable();

        var target = new CoinMarketCapService(new NullLogger<CoinMarketCapService>(), _httpClientFactoryMock.Object);

        var actual = await target.GetInfoAsync(new []{ CryptoCurrencyCode } );

        actual.Should().BeNull();
    }
}