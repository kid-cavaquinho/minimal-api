namespace Exchange.Domain;

public sealed record class Quote(string CurrencyCode, decimal? Price);