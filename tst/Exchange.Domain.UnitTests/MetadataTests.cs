using AutoFixture.Xunit2;
using Xunit;

namespace Exchange.Domain.UnitTests;

public class MetadataTests
{
    [Theory]
    [AutoData]
    public void Should_Throw_ArgumentNullException_When_Code_Is_Null_Or_Empty(int expectedCurrencyId)
    {
        Assert.Throws<ArgumentNullException>(() => new Metadata(expectedCurrencyId, string.Empty));
    }
}