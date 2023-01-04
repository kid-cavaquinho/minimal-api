using System.Text;
using Exchange.Domain.Interfaces;
using Exchange.Infrastructure;
using Exchange.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Exchange.Api.IntegrationTests;

public class DependencyTests
{
    private readonly List<(Type ServiceType, Type? ImplType, ServiceLifetime ServiceLifeTime)> _descriptors = new()
        {
            (typeof(IExchangeService), typeof(CoinMarketCapService), ServiceLifetime.Scoped),
            (typeof(IExchangeService), typeof(ExchangeRatesService), ServiceLifetime.Scoped),
            (typeof(IExchangeServiceFactory), typeof(ExchangeServiceFactory), ServiceLifetime.Scoped),
            // (typeof(Func<ApiSourceType, IExchangeService>), null, ServiceLifetime.Scoped)
        };

    [TestCase]
    public void RegisterValidation()
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(serviceCollection =>
            {
                var services = serviceCollection.ToList();
                
                var result = ValidateServices(services);
                if (result.Success)
                    Assert.Pass();    
                
                Assert.Fail(result.Message);
            });
        });

        factory.CreateClient();
    }

    private DependencyAssertionResult ValidateServices(IReadOnlyCollection<ServiceDescriptor> services)
    {
        var searchFailed = false;
        var failedText = new StringBuilder();
        foreach (var descriptor in _descriptors)
        {
            var match = services.SingleOrDefault(
                x => x.ServiceType == descriptor.ServiceType &&
                     x.Lifetime == descriptor.ServiceLifeTime &&
                     x.ImplementationType == descriptor.ImplType);

            if (match is not null)
                continue;

            if (!searchFailed)
            {
                failedText.AppendLine("Failed to find registered service for:");
                searchFailed = true;
            }

            failedText.AppendLine(
                $"{descriptor.ServiceType.Name}|{descriptor.ImplType?.Name}|{descriptor.ServiceLifeTime}");
        }
        
        return new DependencyAssertionResult
        {
            Success = !searchFailed,
            Message = failedText.ToString()
        };
    }

    internal sealed class DependencyAssertionResult
    {
        public bool Success { get; init; }
        public required string Message { get; init; }
    }
}