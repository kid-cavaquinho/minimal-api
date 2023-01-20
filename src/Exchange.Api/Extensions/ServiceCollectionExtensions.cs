﻿using System.Net;
using System.Net.Mime;
using Exchange.Api.Middlewares;
using Exchange.Api.Modules;
using Exchange.Core.Options;
using Exchange.Core.Ports;
using Exchange.Infrastructure;
using Exchange.Infrastructure.Adapters;
using Exchange.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Exchange.Api.Extensions;

public static class ServiceCollectionExtensions
{
    internal static void AddKernel(this IServiceCollection services)
    {
        services.AddOptions<ApiOptions>().BindConfiguration(nameof(ApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .ValidateOnStart();
        
        services.AddScoped<IExchangeRepositoryFactory, ExchangeRepositoryFactory>();
        services.AddScoped<IExchangeRepository, CoinMarketCapRepository>();
        services.AddScoped<IExchangeRepository, ExchangeRateRepository>();
        
        services.AddOptions<CoinMarketCapApiOptions>().BindConfiguration(nameof(CoinMarketCapApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        services.AddHttpClient(nameof(CoinMarketCapRepository), (sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<CoinMarketCapApiOptions>>().Value;
            httpClient.BaseAddress = options.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json); 
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        
        services.AddOptions<ExchangeRateApiOptions>().BindConfiguration(nameof(ExchangeRateApiOptions),
                options => options.ErrorOnUnknownConfiguration = true)
            .Validate(options => !string.IsNullOrEmpty(options.Key))
            .ValidateOnStart();
        
        services.AddHttpClient(nameof(ExchangeRateRepository), (sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<ExchangeRateApiOptions>>().Value;
            httpClient.BaseAddress = options.BaseAddress;
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json); 
            httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "deflate, gzip");
            httpClient.DefaultRequestHeaders.Add("apikey", options.Key);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
    }

    internal static void AddModules(this IServiceCollection services)
    {
        var modules = typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
        
        foreach (var module in modules)
        {
            module.AddModule(services);
        }
    }

    internal static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Exchange API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "João A.",
                    Email = "joao.antao@protonmail.com",
                    Url = new Uri("https://antao.github.io")
                }
            });
        });
    }

    internal static void AddMiddleware(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
    }
}