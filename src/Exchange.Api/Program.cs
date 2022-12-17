using Exchange.API;
using Exchange.Api.Endpoints;
using Exchange.API.Extensions;
using Exchange.API.Middlewares;
using Exchange.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args).UseSerilog();

builder.Services.AddSwagger();
builder.Services.AddMiddleware();
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

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

try
{
    logger.Information("Starting host");
    await app.RunAsync(app.Lifetime.ApplicationStopped);
    return 0;
}
catch (Exception exception)
{
    logger.Fatal(exception, "Host terminated unexpectedly");
    return 1;
}
finally
{
    await logger.DisposeAsync();
}

// To enable integration tests with `WebApplicationFactory`
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program { }