using System.Text.Json.Serialization;

namespace Exchange.Infrastructure.Services;

public sealed class CoinMarketCapMetadata
{
    [JsonPropertyName("status")] 
    public CoinMarketCapStatus? Status { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, CryptocurrencyMetadataCoinMarketCap[]>? CryptocurrenciesMetadata { get; set; }
}

public sealed class CryptocurrencyMetadataCoinMarketCap
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
        
    [JsonPropertyName("symbol")]
    public required string Symbol { get; set; }
    
    [JsonPropertyName("description")]
    public required string Description { get; set; }
}
