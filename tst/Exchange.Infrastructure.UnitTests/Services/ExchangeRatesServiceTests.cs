using System.Net;
using Exchange.Domain;
using Exchange.Infrastructure.Options;
using Exchange.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Exchange.Infrastructure.UnitTests.Services;

public class ExchangeRatesServiceTests
{
    private const string CryptoCurrencyCode = "TST";
    private readonly IOptions<ExchangeRateApiOptions> _options;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public ExchangeRatesServiceTests()
    {
        _options = Microsoft.Extensions.Options.Options.Create(new ExchangeRateApiOptions
        {
            Key = "a-key", 
            BaseAddress = new Uri("http://localhost/")
        });
        
        _httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    }
    
    [Fact]
    public async Task GetQuotesAsync_Should_Return_Empty_Array_Quotes_When_Deserialization_Fails()
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("deserialization will fail")
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(ExchangeRatesService))).Returns(httpClient).Verifiable();

        var target = new ExchangeRatesService(_httpClientFactoryMock.Object, _options, new NullLogger<ExchangeRatesService>());

        var actual = await target.GetQuotesAsync(CryptoCurrencyCode);

        actual.Should().NotBeNull();
        actual.As<CryptoCurrencyQuote>().CryptoCurrencyCode.Should().Be(CryptoCurrencyCode);
        actual.As<CryptoCurrencyQuote>().Quotes.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetQuotesAsync_Should_Return_Empty_Array_Quotes_When_Rates_Are_Null()
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"base\": \"TST\",\n  \"date\": \"2022-12-11\",\n  \"success\": true,\n  \"timestamp\": 1670796483\n}")
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(ExchangeRatesService))).Returns(httpClient).Verifiable();

        var target = new ExchangeRatesService(_httpClientFactoryMock.Object, _options, new NullLogger<ExchangeRatesService>());

        var actual = await target.GetQuotesAsync(CryptoCurrencyCode);

        actual.Should().NotBeNull();
        actual.As<CryptoCurrencyQuote>().CryptoCurrencyCode.Should().Be(CryptoCurrencyCode);
        actual.As<CryptoCurrencyQuote>().Quotes.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetQuotesAsync_Should_Return_Empty_Array_Quotes_When_Response_Is_ServiceUnavailable()
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(ExchangeRatesService))).Returns(httpClient).Verifiable();

        var target = new ExchangeRatesService(_httpClientFactoryMock.Object, _options, new NullLogger<ExchangeRatesService>());

        var actual = await target.GetQuotesAsync(CryptoCurrencyCode);

        actual.Should().NotBeNull();
        actual.As<CryptoCurrencyQuote>().CryptoCurrencyCode.Should().Be(CryptoCurrencyCode);
        actual.As<CryptoCurrencyQuote>().Quotes.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetQuotesAsync_Should_Return_CrytoCurrencyQuote_With_5_Quotes()
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"base\": \"BTC\",\n  \"date\": \"2022-12-11\",\n  \"rates\": {\n    \"AUD\": 25222.834753,\n    \"BRL\": 89699.792225,\n    \"EUR\": 16256.868149,\n    \"GBP\": 13962.367343,\n    \"USD\": 17124.8407\n  },\n  \"success\": true,\n  \"timestamp\": 1670796483\n}"),
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(ExchangeRatesService))).Returns(httpClient).Verifiable();

        var target = new ExchangeRatesService(_httpClientFactoryMock.Object, _options, new NullLogger<ExchangeRatesService>());

        var actual = await target.GetQuotesAsync(CryptoCurrencyCode);

        actual.Should().NotBeNull();
        actual.As<CryptoCurrencyQuote>().CryptoCurrencyCode.Should().Be(CryptoCurrencyCode);
        actual.As<CryptoCurrencyQuote>().Quotes.Should().HaveCount(5);
    }
    
    [Fact]
    public async Task GetQuotesAsync_Should_Return_Empty_Array_Quotes_When_TaskCanceledException_Is_Thrown()
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException())
            .Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(ExchangeRatesService))).Returns(httpClient).Verifiable();

        var target = new ExchangeRatesService(_httpClientFactoryMock.Object, _options, new NullLogger<ExchangeRatesService>());

        var actual = await target.GetQuotesAsync(CryptoCurrencyCode);

        actual.Should().NotBeNull();
        actual.As<CryptoCurrencyQuote>().CryptoCurrencyCode.Should().Be(CryptoCurrencyCode);
        actual.As<CryptoCurrencyQuote>().Quotes.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetInfoAsync_Should_Throw_Not_Implemented_Exception()
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(ExchangeRatesService))).Returns(httpClient).Verifiable();
        var target = new ExchangeRatesService(_httpClientFactoryMock.Object, _options, new NullLogger<ExchangeRatesService>());
        Func<Task> act = async () => { await target.GetInfoAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()); };
        await act.Should().ThrowAsync<NotImplementedException>();
    }
}