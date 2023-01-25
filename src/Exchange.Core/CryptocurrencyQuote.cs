namespace Exchange.Core;

public sealed class CryptocurrencyQuote
{
    public CryptocurrencyQuote(string cryptocurrencySymbol, IEnumerable<Quote> quotes)
    {
        if (string.IsNullOrEmpty(cryptocurrencySymbol))
            throw new ArgumentNullException(nameof(cryptocurrencySymbol));

        CryptocurrencySymbol = cryptocurrencySymbol;
        Quotes = quotes;
    }

    public string CryptocurrencySymbol { get; }

    public IEnumerable<Quote> Quotes { get; }
}