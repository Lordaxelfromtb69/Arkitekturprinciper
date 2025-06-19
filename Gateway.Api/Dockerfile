# Trin 1: Brug officiel .NET SDK-billede til at bygge applikationen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Trin 2: Kopiér csproj-filer (inkl. Shared) til korrekte undermapper
COPY Gateway.Api/Gateway.Api.csproj ./Gateway.Api/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Gateway.Api/Gateway.Api.csproj

# Trin 3: Kopiér resten af kildekoden
COPY Gateway.Api/ ./Gateway.Api/
COPY Shared/ ./Shared/

# Trin 4: Byg og publish Gateway.Api-projektet
RUN dotnet publish Gateway.Api/Gateway.Api.csproj -c Release -o /out

# Trin 5: Runtime image med ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Trin 6: Kopiér bygget output ind i runtime-containeren
COPY --from=build /out ./

# Trin 7: Eksponér port og definér opstart
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
