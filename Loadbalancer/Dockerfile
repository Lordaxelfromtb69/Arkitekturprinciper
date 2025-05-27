# Trin 1: Brug .NET SDK som build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Trin 2: Kopiér csproj-filer
COPY Loadbalancer/Loadbalancer.csproj ./Loadbalancer/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Loadbalancer/Loadbalancer.csproj

# Trin 3: Kopiér hele kildekoden
COPY Loadbalancer/ ./Loadbalancer/
COPY Shared/ ./Shared/

# Trin 4: Publish projektet til /out
RUN dotnet publish Loadbalancer/Loadbalancer.csproj -c Release -o /out

# Trin 5: Brug runtime-image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Trin 6: Kopiér publiceret output
COPY --from=build /out ./

# Trin 7: Eksponér port og definer entrypoint
EXPOSE 80
ENTRYPOINT ["dotnet", "Loadbalancer.dll"]
