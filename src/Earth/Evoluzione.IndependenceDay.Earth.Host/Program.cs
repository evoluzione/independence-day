using Evoluzione.IndependenceDay.Earth.Facade;
using Evoluzione.IndependenceDay.Earth.Facade.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEarth(builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapBattleEndpoints();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "earth" }));

app.Logger.LogInformation("[Earth] Servizio avviato: la sala operativa e' su /");

await app.RunAsync();
