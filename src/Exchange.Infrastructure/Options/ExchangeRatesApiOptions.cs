namespace Exchange.Infrastructure.Options;

internal sealed class ExchangeRatesApiOptions
{
    public required Uri BaseAddress { get; set; }

    public required string Key { get; set; }
}