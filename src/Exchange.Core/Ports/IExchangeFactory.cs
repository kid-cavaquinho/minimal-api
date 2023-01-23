namespace Exchange.Core.Ports;

public interface IExchangeFactory
{
    IExchangeRepository GetInstance(); 
}