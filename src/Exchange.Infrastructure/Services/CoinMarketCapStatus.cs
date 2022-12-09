using System.Text.Json.Serialization;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapStatus
{
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonPropertyName("error_code")]
    public long ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("elapsed")]
    public long? Elapsed { get; set; }

    [JsonPropertyName("credit_count")]
    public long? CreditCount { get; set; }

    [JsonPropertyName("notice")]
    public object? Notice { get; set; }
}