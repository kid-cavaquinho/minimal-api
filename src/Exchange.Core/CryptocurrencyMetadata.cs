namespace Exchange.Core;

public sealed class CryptocurrencyMetadata
{
    public CryptocurrencyMetadata(int id, string symbol, string description)
    {
        Id = id;
        Symbol = symbol;
        Description = description;
    }

    public int Id { get; }

    public string Symbol { get; }

    public string Description { get; }
}
