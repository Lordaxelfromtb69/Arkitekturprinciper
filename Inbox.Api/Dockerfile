# 1. Brug .NET SDK til at bygge
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. Kopiér projektfil og Shared afhængighed
COPY Inbox.Api/Inbox.Api.csproj ./Inbox.Api/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Inbox.Api/Inbox.Api.csproj

# 3. Kopiér al kildekode
COPY Inbox.Api/ ./Inbox.Api/
COPY Shared/ ./Shared/

# 4. Build og publish
RUN dotnet publish Inbox.Api/Inbox.Api.csproj -c Release -o /out

# 5. Brug .NET runtime til at køre
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "Inbox.Api.dll"]
