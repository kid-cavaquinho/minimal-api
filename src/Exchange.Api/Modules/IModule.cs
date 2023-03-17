namespace Exchange.Api.Modules;

// Todo: Consider move to Core project
public interface IModule
{
    void AddModule(IServiceCollection services);
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}