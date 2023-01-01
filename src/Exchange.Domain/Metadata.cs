namespace Exchange.Domain;

public sealed class Metadata
{
    public Metadata(int id, string symbol, string description)
    {
        Id = id;
        Symbol = symbol;
        Description = description;
    }

    public int Id { get; }

    public string Symbol { get; }

    public string Description { get; }
}
