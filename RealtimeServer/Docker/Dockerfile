# Dotnet Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Dotnet SDK Image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RealtimeServer/RealtimeServer.csproj", "RealtimeServer/"]
RUN dotnet restore "RealtimeServer/RealtimeServer.csproj"
COPY . .
WORKDIR "/src/RealtimeServer"
RUN dotnet build "RealtimeServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealtimeServer.dll"]