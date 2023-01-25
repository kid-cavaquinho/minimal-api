minimal-api
=================

## Introduction

This sample project is build using the [minimal APIs] feature approach and demonstrates:
* A project structure for your application. 
* Techniques you can use for integration testing an ASP.NET Core 7 web application.

Minimal APIs are a simplified approach for building fast HTTP APIs with ASP.NET Core. You can build fully functioning REST endpoints with minimal code and configuration.

[minimal APIs]: https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/#introducing-minimal-apis

These endpoints rely on two data sources: 
* [CoinMarketCap]
* [ExchangeRates]

By default, it uses CoinMarketCap as data source.

[CoinMarketCap]: https://coinmarketcap.com/api/
[ExchangeRates]: https://exchangeratesapi.io/

The tests available in the `tst` folder show how you can write unit and integration tests to help you get coverage of the
system-under-test, as well as give confidence that the changes you make
to an application are ready to ship to a production system.

## Building and Testing

Compiling the application yourself requires Git and the
[.NET SDK](https://www.microsoft.com/net/download/core "Download the .NET SDK")
to be installed (version `7.0.100` or later).

## Debugging

To debug the application locally outside of the integration tests, you will need
to create API keys to replace secrets for the `ExchangeRatesApiOptions:Key` either-or
`CoinMarketCapApiOptions:Key` so that the services work properly. You can create free keys with limited usage.

> ⚠️ Do not commit secrets to source control. Configure them with [user secrets] instead.

[User Secrets]: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets

## Docker
This solution can be dockerized, install a suitable version of docker for your computer/architecture and run the following command:

```
docker build --tag "exchange-api" --file "Dockerfile" .
docker run -p 8080:80 --rm --detach --name joao-assignment exchange-api 
```