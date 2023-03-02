#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/VideoManager/VideoManager.SignalRHub/VideoManager.SignalRHub.csproj", "Services/VideoManager/VideoManager.SignalRHub/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["Services/VideoManager/VideoManager.Domain/VideoManager.Domain.csproj", "Services/VideoManager/VideoManager.Domain/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
RUN dotnet restore "Services/VideoManager/VideoManager.SignalRHub/VideoManager.SignalRHub.csproj"
COPY . .
WORKDIR "/src/Services/VideoManager/VideoManager.SignalRHub"
RUN dotnet build "VideoManager.SignalRHub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoManager.SignalRHub.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoManager.SignalRHub.dll"]