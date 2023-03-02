#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Storage/Storage.API/Storage.API.csproj", "Services/Storage/Storage.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["Services/Storage/Storage.Domain/Storage.Domain.csproj", "Services/Storage/Storage.Domain/"]
COPY ["Services/Storage/Storage.Infrastructure/Storage.Infrastructure.csproj", "Services/Storage/Storage.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.EFCore/Infrastructure.EFCore.csproj", "BuildingBlocks/Infrastructure.EFCore/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["Services/Storage/Storage.Shared/Storage.Shared.csproj", "Services/Storage/Storage.Shared/"]
RUN dotnet restore "Services/Storage/Storage.API/Storage.API.csproj"
COPY . .
WORKDIR "/src/Services/Storage/Storage.API"
RUN dotnet build "Storage.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Storage.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Storage.API.dll"]