namespace Exchange.Core.Ports;

public interface IExchangeRepositoryFactory
{
    IExchangeRepository GetInstance(); 
}