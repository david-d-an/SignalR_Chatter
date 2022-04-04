#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Please read DockerInstructions.md to build and upload a docker image defined in this file.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RealtimeServer.SignalR/RealtimeServer.SignalR.csproj", "RealtimeServer.SignalR/"]
RUN dotnet restore "RealtimeServer.SignalR/RealtimeServer.SignalR.csproj"
COPY . .
WORKDIR "/src/RealtimeServer.SignalR"
RUN dotnet build "RealtimeServer.SignalR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeServer.SignalR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealtimeServer.SignalR.dll"]