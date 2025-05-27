# Trin 1: Brug .NET SDK til at bygge applikationen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Trin 2: Kopiér csproj-filer
COPY Deleted.Api/Deleted.Api.csproj ./Deleted.Api/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Deleted.Api/Deleted.Api.csproj

# Trin 3: Kopiér hele kildekoden
COPY Deleted.Api/ ./Deleted.Api/
COPY Shared/ ./Shared/

# Trin 4: Byg og publish
RUN dotnet publish Deleted.Api/Deleted.Api.csproj -c Release -o /out

# Trin 5: Runtime-billede
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Trin 6: Kopiér build output fra build-step
COPY --from=build /out ./

# Trin 7: Konfigurer port og entrypoint
EXPOSE 80
ENTRYPOINT ["dotnet", "Deleted.Api.dll"]
