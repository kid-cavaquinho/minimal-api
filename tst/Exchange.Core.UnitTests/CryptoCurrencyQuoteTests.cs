using AutoFixture.Xunit2;
using Xunit;

namespace Exchange.Core.UnitTests;

public class CryptocurrencyQuoteTests
{
    [Theory]
    [AutoData]
    public void Should_Throw_ArgumentNullException_When_Code_Is_Null_Or_Empty(IEnumerable<Quote> expectedQuotes)
    {
        Assert.Throws<ArgumentNullException>(() => new CryptocurrencyQuote(string.Empty, expectedQuotes));
    }
}