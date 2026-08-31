# SPREEK POORT 8080 AAN (De basissfeer voor de uiteindelijke app)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# BOUW DE APPLICATIE (Gebruik de SDK image)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopieer het projectbestand en herstel de NuGet-pakketten
# Pas "MijnApp.csproj" aan naar de echte naam van jouw .csproj bestand!
COPY ["bestelplatform.csproj", "./"]
RUN dotnet restore "./bestelplatform.csproj"

# Kopieer de rest van de broncode en bouw het project
COPY . .
RUN dotnet build "bestelplatform.csproj" -c Release -o /app/build

# PUBLICEER DE APPLICATIE
FROM build AS publish
RUN dotnet publish "bestelplatform.csproj" -c Release -o /app/publish /p:UseAppHost=false

# DEFINIEER DE STARTCONTAINER
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Pas "MijnApp.dll" aan naar de echte naam van jouw gegenereerde dll bestand!
ENTRYPOINT ["dotnet", "bestelplatform.dll"]