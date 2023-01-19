using Asp.Versioning.Builder;

namespace Exchange.Api.Modules;

public interface IModule
{
    void AddModule(IServiceCollection services);
    void MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet apiVersionSet);
}