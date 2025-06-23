FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Trin 1: Kopiér csproj-filer og restore
COPY ["Inbox.Api/Inbox.Api.csproj", "Inbox.Api/"]
RUN dotnet restore "Inbox.Api/Inbox.Api.csproj"

# Trin 2: Kopiér resten af koden
COPY . .

# Trin 3: Byg projektet
WORKDIR "/src/Inbox.Api"
RUN dotnet build "Inbox.Api.csproj" -c Release -o /app/build

# Trin 4: Publicér
FROM build AS publish
RUN dotnet publish "Inbox.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Trin 5: Runtime
FROM base AS final
WORKDIR /app

# Trin 6: Kopiér publiceret kode og data
COPY --from=publish /app/publish .
COPY Inbox.Api/inbox /app/inbox

# Trin 7: Start API
ENTRYPOINT ["dotnet", "Inbox.Api.dll"]
