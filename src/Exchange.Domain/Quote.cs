namespace Exchange.Domain;

public readonly record struct Quote(string CurrencyCode, decimal? Price);