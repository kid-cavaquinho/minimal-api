using Exchange.Api.Extensions;

var builder = WebApplication.CreateBuilder(args).UseSerilog();

builder.Services.AddSwagger();
builder.Services.AddKernel();
builder.Services.AddModules();

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

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => await Results.Problem().ExecuteAsync(context)));
app.MapEndpoints();

await app.RunAsync(app.Lifetime.ApplicationStopped);

// To enable integration tests with `WebApplicationFactory`
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program { }