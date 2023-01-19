namespace Exchange.Core;

public sealed record class Quote(string CurrencyCode, decimal? Price);