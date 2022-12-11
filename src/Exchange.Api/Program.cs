using Exchange.API;
using Exchange.Api.Endpoints;
using Exchange.API.Extensions;
using Exchange.API.Middlewares;
using Exchange.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args).UseSerilog();

builder.Services.AddSwagger();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddInfrastructure();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseMetadataEndpoint();
app.UseQuotesEndpoint();

try
{
    Log.Information("Starting host");
    await app.RunAsync(app.Lifetime.ApplicationStopped);
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

// To enable integration tests with `WebApplicationFactory`
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program { }