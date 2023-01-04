namespace Exchange.Domain.Interfaces;

public interface IExchangeServiceFactory
{
    IExchangeService GetInstance(ApiSourceType type); 
}