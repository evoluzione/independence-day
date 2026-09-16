using Evoluzione.IndependenceDay.Sagas.Facade;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSagas(builder.Configuration);

var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "defense-saga" }));

app.Logger.LogInformation("[Saga] Coordinamento della difesa in ascolto.");

await app.RunAsync();
