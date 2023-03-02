#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/VideoManager/VideoManager.API/VideoManager.API.csproj", "Services/VideoManager/VideoManager.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.Helper/EventBus.Helper.csproj", "BuildingBlocks/EventBus.Helper/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["Services/Storage/Storage.Shared/Storage.Shared.csproj", "Services/Storage/Storage.Shared/"]
COPY ["Services/VideoManager/VideoManager.Domain/VideoManager.Domain.csproj", "Services/VideoManager/VideoManager.Domain/"]
COPY ["Services/VideoManager/VideoManager.Infrastructure/VideoManager.Infrastructure.csproj", "Services/VideoManager/VideoManager.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.EFCore/Infrastructure.EFCore.csproj", "BuildingBlocks/Infrastructure.EFCore/"]
RUN dotnet restore "Services/VideoManager/VideoManager.API/VideoManager.API.csproj"
COPY . .
WORKDIR "/src/Services/VideoManager/VideoManager.API"
RUN dotnet build "VideoManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoManager.API.dll"]