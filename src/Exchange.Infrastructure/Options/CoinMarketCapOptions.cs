namespace Exchange.Infrastructure.Options;

internal sealed class CoinMarketCapApiOptions
{
    public required Uri BaseAddress { get; set; }

    public required string Key { get; set; }
}