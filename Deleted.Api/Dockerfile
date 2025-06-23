# Trin 1: Brug .NET SDK til at bygge applikationen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Trin 2: Kopiér csproj-filer og restore afhængigheder
COPY Deleted.Api/Deleted.Api.csproj ./Deleted.Api/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Deleted.Api/Deleted.Api.csproj

# Trin 3: Kopiér hele kildekoden
COPY Deleted.Api/ ./Deleted.Api/
COPY Shared/ ./Shared/

# Trin 4: Byg og publish til /out
RUN dotnet publish Deleted.Api/Deleted.Api.csproj -c Release -o /out

# Trin 5: Runtime-image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Trin 6: Kopiér build output og data
COPY --from=build /out ./
COPY Deleted.Api/deleted_items /app/deleted_items

# Trin 7: Eksponér port og start API
EXPOSE 80
ENTRYPOINT ["dotnet", "Deleted.Api.dll"]
