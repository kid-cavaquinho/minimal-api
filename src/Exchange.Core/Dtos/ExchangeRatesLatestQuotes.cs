using System.Text.Json.Serialization;

namespace Exchange.Core.Dtos;

public sealed class ExchangeRateLatestQuotes
{
    [JsonPropertyName("rates")] 
    public Dictionary<string, decimal>? Rates { set; get; }

    [JsonPropertyName("base")]
    public string? BaseCurrency { set; get; }

    [JsonPropertyName("timestamp")] 
    public long Timestamp { get; set; }
}