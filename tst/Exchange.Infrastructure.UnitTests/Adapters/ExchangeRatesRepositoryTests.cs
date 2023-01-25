using Exchange.Core;
using Exchange.Infrastructure.Adapters;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Exchange.Infrastructure.UnitTests.Adapters;

public class ExchangeRatesRepositoryTests
{
    private readonly ExchangeRatesRepository _sut;

    public ExchangeRatesRepositoryTests()
    {
        Mock<IHttpClientFactory> httpClientFactoryMock = new();
        _sut = new ExchangeRatesRepository(new NullLoggerFactory(), httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task GetMetadataAsync_Should_Throw_NotImplementedException()
    {
        await Assert.ThrowsAsync<NotImplementedException>(async () => await _sut.GetMetadataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
    }
}