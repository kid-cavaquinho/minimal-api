﻿namespace Exchange.Core.Options;

public class ExchangeRatesApiOptions
{
    public required Uri BaseAddress { get; set; }

    public required string Key { get; set; }
}