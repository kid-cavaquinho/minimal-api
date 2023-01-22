using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Exchange.Core.Ports.DTOs;

public sealed class CoinMarketCapLatestQuotes
{
    [JsonPropertyName("data")] 
    public required JsonNode Data { get; set; }
}

public sealed class CoinMarketCapQuotesLatestResponseV2
{
    [JsonPropertyName("status")] 
    public CoinMarketCapStatus? Status { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, CoinMarketCapCryptocurrency>? Cryptocurrencies { get; set; }
}

public sealed class CoinMarketCapCryptocurrency
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }
        
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; } 
        
    [JsonPropertyName("quote")]
    public Dictionary<string, CoinMarketCapQuote>? Quote { get; set; }
}

public sealed class CoinMarketCapQuote
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("last_updated")]
    public DateTimeOffset LastUpdated { get; set; }
}