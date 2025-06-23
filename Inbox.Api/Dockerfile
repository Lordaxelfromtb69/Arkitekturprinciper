FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Inbox.Api/Inbox.Api.csproj", "Inbox.Api/"]
RUN dotnet restore "Inbox.Api/Inbox.Api.csproj"
COPY . .
WORKDIR "/src/Inbox.Api"
RUN dotnet build "Inbox.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Inbox.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY inbox /app/inbox
ENTRYPOINT ["dotnet", "Inbox.Api.dll"]
