# Build e runtime stanno entrambi qui dentro: chi clona il repository non deve installare .NET.
# Il contesto di build e' src/, cosi' il restore vede tutti i progetti referenziati.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Earth/Evoluzione.IndependenceDay.Earth.Host/Evoluzione.IndependenceDay.Earth.Host.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Evoluzione.IndependenceDay.Earth.Host.dll"]
