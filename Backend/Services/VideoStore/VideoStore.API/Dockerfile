#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/VideoStore/VideoStore.API/VideoStore.API.csproj", "Services/VideoStore/VideoStore.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.Helper/EventBus.Helper.csproj", "BuildingBlocks/EventBus.Helper/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["Services/Storage/Storage.Shared/Storage.Shared.csproj", "Services/Storage/Storage.Shared/"]
COPY ["Services/VideoStore/VideoStore.Domain/VideoStore.Domain.csproj", "Services/VideoStore/VideoStore.Domain/"]
COPY ["Services/VideoStore/VideoStore.Infrastructure/VideoStore.Infrastructure.csproj", "Services/VideoStore/VideoStore.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.EFCore/Infrastructure.EFCore.csproj", "BuildingBlocks/Infrastructure.EFCore/"]
RUN dotnet restore "Services/VideoStore/VideoStore.API/VideoStore.API.csproj"
COPY . .
WORKDIR "/src/Services/VideoStore/VideoStore.API"
RUN dotnet build "VideoStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoStore.API.dll"]