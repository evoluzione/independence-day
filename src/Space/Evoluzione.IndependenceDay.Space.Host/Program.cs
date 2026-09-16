using Evoluzione.IndependenceDay.Space.Facade;
using Evoluzione.IndependenceDay.Space.Facade.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSpace(builder.Configuration);

var app = builder.Build();
app.MapShipsEndpoints();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "space" }));

app.Logger.LogInformation("[Space] Servizio avviato: lo spazio e' in ascolto.");

await app.RunAsync();
