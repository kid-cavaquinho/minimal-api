Consider
-- Use default interface methods (C# version 8 feature)
-- Add cache to `ExchangeRatesService` class
-- Modify cache results (currently caches the HTTP byte[] response) on `CoinMarketCapService` to avoid allocations and improve performance

// Todo: Benchmark performance
// Todo: Add unit and integration tests
// Todo: Add ports/adaptors
// Todo: Resilience and error messages
