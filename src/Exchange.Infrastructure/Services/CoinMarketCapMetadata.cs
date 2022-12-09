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
    public string? Symbol { get; set; } 
    
    [JsonPropertyName("name")]
    public string? Name { get; set; } 
    
    [JsonPropertyName("slug")]
    public string? Slug { get; set; } 
}
