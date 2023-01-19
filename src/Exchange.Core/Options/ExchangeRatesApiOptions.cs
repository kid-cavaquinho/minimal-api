namespace Exchange.Core.Options;

public class ExchangeRateApiOptions
{
    public required Uri BaseAddress { get; set; }

    public required string Key { get; set; }
}