namespace Exchange.Domain;

public sealed class CryptoCurrencyQuote
{
    public CryptoCurrencyQuote(string cryptoCurrencyCode, IEnumerable<Quote> quotes)
    {
        if (string.IsNullOrWhiteSpace(cryptoCurrencyCode))
            throw new ArgumentNullException(nameof(cryptoCurrencyCode));

        CryptoCurrencyCode = cryptoCurrencyCode;
        Quotes = quotes;
    }

    public string CryptoCurrencyCode { get; }

    public IEnumerable<Quote> Quotes { get; }
}