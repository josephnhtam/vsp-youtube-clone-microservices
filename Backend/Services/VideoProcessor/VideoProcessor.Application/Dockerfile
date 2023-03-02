#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

RUN apt-get update && apt-get install -y ffmpeg

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/VideoProcessor/VideoProcessor.Application/VideoProcessor.Application.csproj", "Services/VideoProcessor/VideoProcessor.Application/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["Services/Storage/Storage.Shared/Storage.Shared.csproj", "Services/Storage/Storage.Shared/"]
COPY ["Services/VideoProcessor/VideoProcessor.Domain/VideoProcessor.Domain.csproj", "Services/VideoProcessor/VideoProcessor.Domain/"]
COPY ["Services/VideoProcessor/VideoProcessor.Infrastructure/VideoProcessor.Infrastructure.csproj", "Services/VideoProcessor/VideoProcessor.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.EFCore/Infrastructure.EFCore.csproj", "BuildingBlocks/Infrastructure.EFCore/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
RUN dotnet restore "Services/VideoProcessor/VideoProcessor.Application/VideoProcessor.Application.csproj"
COPY . .
WORKDIR "/src/Services/VideoProcessor/VideoProcessor.Application"
RUN dotnet build "VideoProcessor.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoProcessor.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "VideoProcessor.Application.dll"]