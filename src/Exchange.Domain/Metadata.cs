namespace Exchange.Domain;

public sealed class Metadata
{
    public Metadata(int currencyId, string? cryptoCurrencyCode)
    {
        if (string.IsNullOrWhiteSpace(cryptoCurrencyCode))
            throw new ArgumentNullException(nameof(cryptoCurrencyCode));
        
        CurrencyId = currencyId;
        CryptoCurrencyCode = cryptoCurrencyCode;
    }

    public int CurrencyId { get; }

    public string CryptoCurrencyCode { get; }
}