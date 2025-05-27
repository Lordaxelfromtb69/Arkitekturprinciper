# Trin 1: Brug .NET SDK til at bygge
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Trin 2: Kopiér .csproj og Shared afhængigheder
COPY Sent2.Api/Sent2.Api.csproj ./Sent2.Api/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Sent2.Api/Sent2.Api.csproj

# Trin 3: Kopiér hele projektkoden
COPY Sent2.Api/ ./Sent2.Api/
COPY Shared/ ./Shared/

# Trin 4: Publicér projektet
RUN dotnet publish Sent2.Api/Sent2.Api.csproj -c Release -o /out

# Trin 5: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Trin 6: Eksponér port og start API
EXPOSE 80
ENTRYPOINT ["dotnet", "Sent2.Api.dll"]
