# Trin 1: Brug .NET SDK til at bygge
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Trin 2: Kopiér .csproj og restore afhængigheder
COPY Sent1.Api/Sent1.Api.csproj ./Sent1.Api/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Sent1.Api/Sent1.Api.csproj

# Trin 3: Kopiér den fulde kode
COPY Sent1.Api/ ./Sent1.Api/
COPY Shared/ ./Shared/

# Trin 4: Publicér build til /out
RUN dotnet publish Sent1.Api/Sent1.Api.csproj -c Release -o /out

# Trin 5: Runtime-image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Trin 6: Kopiér publiceret output og data til runtime container
COPY --from=build /out ./
COPY Sent1.Api/_sent_mail /app/_sent_mail

# Trin 7: Start applikationen
EXPOSE 80
ENTRYPOINT ["dotnet", "Sent1.Api.dll"]
