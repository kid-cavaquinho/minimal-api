using System.Text.Json.Serialization;

namespace Exchange.Core.Ports.DTOs;

public sealed record CoinMarketCapMetadata
{
    [JsonPropertyName("data")]
    public Dictionary<string, CryptocurrencyMetadataCoinMarketCap[]>? CryptocurrenciesMetadata { get; init; }
}

public sealed record CryptocurrencyMetadataCoinMarketCap
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
        
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }
    
    [JsonPropertyName("description")]
    public required string Description { get; init; }
}
