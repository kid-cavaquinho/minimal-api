namespace Exchange.Domain;

public sealed class Metadata
{
    public Metadata(int id, string? symbol, string description)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentNullException(nameof(symbol));
        
        Id = id;
        Symbol = symbol;
        Description = description;
    }

    public int Id { get; }

    public string Symbol { get; }

    public string Description { get; }
}
