namespace Exchange.Infrastructure.Services;

public static class CoinMarketCapCurrencyId
{
    public static readonly (int Id, string Name) Usd = new(2781, "USD");
    public static readonly (int Id, string Name) Aud = new(2782, "AUD");
    public static readonly (int Id, string Name) Blr = new(2783, "BLR");
    public static readonly (int Id, string Name) Eur = new(2790, "EUR");
    public static readonly (int Id, string Name) Gbp = new(2791, "GPB");
}