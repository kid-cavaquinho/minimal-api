using AutoFixture.Xunit2;
using Moq;
using Xunit;

namespace Exchange.Domain.UnitTests;

public class MetadataTests
{
    [Theory]
    [AutoData]
    public void Should_Throw_ArgumentNullException_When_Symbol_Is_Null_Or_Empty(int expectedCurrencyId, string expectedDescription)
    {
        Assert.Throws<ArgumentNullException>(() => new Metadata(expectedCurrencyId, string.Empty, expectedDescription));
    }
}