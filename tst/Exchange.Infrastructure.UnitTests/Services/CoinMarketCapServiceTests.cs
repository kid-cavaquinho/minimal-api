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
    public async Task GetInfoAsync_Should_Return_Null_When_Deserialization_Fails()
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
    
        var actual = await target.GetInfoAsync(new CryptoCurrencySymbol(CryptoCurrencyCode));
    
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetInfoAsync_Should_Return_Null_When_Metadata_Is_Not_Found()
    {
        const string expectedCurrencySymbol = "ETH";
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        httpClient.BaseAddress = _options.Value.BaseAddress;
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(CoinMarketCapService))).Returns(httpClient).Verifiable();
    
        var target = new CoinMarketCapService(new NullLogger<CoinMarketCapService>(), _httpClientFactoryMock.Object);
    
        var actual = await target.GetInfoAsync(new CryptoCurrencySymbol(expectedCurrencySymbol));

        actual.As<Metadata>().Should().BeNull();
    }
    
    [Fact]
    public async Task GetInfoAsync_Should_Return_Valid_Metadata()
    {
        const string expectedCurrencySymbol = "BTC";
        const string response = @"{""status"":{""timestamp"":""2023-01-01T19:05:38.832Z"",""error_code"":0,""error_message"":null,""elapsed"":18,""credit_count"":1,""notice"":null},""data"":{""BTC"":[{""id"":1,""name"":""Bitcoin"",""symbol"":""BTC"",""category"":""coin"",""description"":""Bitcoin (BTC) is a cryptocurrency . Users are able to generate BTC through the process of mining. Bitcoin has a current supply of 19,249,400. The last known price of Bitcoin is 16,596.07245705 USD and is up 0.09 over the last 24 hours. It is currently trading on 9914 active market(s) with $9,452,161,133.80 traded over the last 24 hours. More information can be found at https://bitcoin.org/."",""slug"":""bitcoin"",""logo"":""https://s2.coinmarketcap.com/static/img/coins/64x64/1.png"",""subreddit"":""bitcoin"",""notice"":"""",""tags"":[""mineable"",""pow"",""sha-256"",""store-of-value"",""state-channel"",""coinbase-ventures-portfolio"",""three-arrows-capital-portfolio"",""polychain-capital-portfolio"",""binance-labs-portfolio"",""blockchain-capital-portfolio"",""boostvc-portfolio"",""cms-holdings-portfolio"",""dcg-portfolio"",""dragonfly-capital-portfolio"",""electric-capital-portfolio"",""fabric-ventures-portfolio"",""framework-ventures-portfolio"",""galaxy-digital-portfolio"",""huobi-capital-portfolio"",""alameda-research-portfolio"",""a16z-portfolio"",""1confirmation-portfolio"",""winklevoss-capital-portfolio"",""usv-portfolio"",""placeholder-ventures-portfolio"",""pantera-capital-portfolio"",""multicoin-capital-portfolio"",""paradigm-portfolio""],""tag-names"":[""Mineable"",""PoW"",""SHA-256"",""Store Of Value"",""State Channel"",""Coinbase Ventures Portfolio"",""Three Arrows Capital Portfolio"",""Polychain Capital Portfolio"",""Binance Labs Portfolio"",""Blockchain Capital Portfolio"",""BoostVC Portfolio"",""CMS Holdings Portfolio"",""DCG Portfolio"",""DragonFly Capital Portfolio"",""Electric Capital Portfolio"",""Fabric Ventures Portfolio"",""Framework Ventures Portfolio"",""Galaxy Digital Portfolio"",""Huobi Capital Portfolio"",""Alameda Research Portfolio"",""a16z Portfolio"",""1Confirmation Portfolio"",""Winklevoss Capital Portfolio"",""USV Portfolio"",""Placeholder Ventures Portfolio"",""Pantera Capital Portfolio"",""Multicoin Capital Portfolio"",""Paradigm Portfolio""],""tag-groups"":[""OTHERS"",""ALGORITHM"",""ALGORITHM"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY"",""CATEGORY""],""urls"":{""website"":[""https://bitcoin.org/""],""twitter"":[],""message_board"":[""https://bitcointalk.org""],""chat"":[],""facebook"":[],""explorer"":[""https://blockchain.coinmarketcap.com/chain/bitcoin"",""https://blockchain.info/"",""https://live.blockcypher.com/btc/"",""https://blockchair.com/bitcoin"",""https://explorer.viabtc.com/btc""],""reddit"":[""https://reddit.com/r/bitcoin""],""technical_doc"":[""https://bitcoin.org/bitcoin.pdf""],""source_code"":[""https://github.com/bitcoin/bitcoin""],""announcement"":[]},""platform"":null,""date_added"":""2013-04-28T00:00:00.000Z"",""twitter_username"":"""",""is_hidden"":0,""date_launched"":null,""contract_address"":[],""self_reported_circulating_supply"":null,""self_reported_tags"":null,""self_reported_market_cap"":null}]}}";
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(response)
            }).Verifiable();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        httpClient.BaseAddress = _options.Value.BaseAddress;
        _httpClientFactoryMock.Setup(_ => _.CreateClient(nameof(CoinMarketCapService))).Returns(httpClient).Verifiable();
    
        var target = new CoinMarketCapService(new NullLogger<CoinMarketCapService>(), _httpClientFactoryMock.Object);
    
        var actual = await target.GetInfoAsync(new CryptoCurrencySymbol(expectedCurrencySymbol));

        actual.As<Metadata>().Should().NotBeNull();
        actual.As<Metadata>().Id.Should().Be(1);
        actual.As<Metadata>().Symbol.Should().Be(expectedCurrencySymbol);
    }
}