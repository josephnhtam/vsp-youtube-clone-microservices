#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Search/Search.API/Search.API.csproj", "Services/Search/Search.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["Services/Search/Search.Domain/Search.Domain.csproj", "Services/Search/Search.Domain/"]
COPY ["Services/Search/Search.Infrastructure/Search.Infrastructure.csproj", "Services/Search/Search.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.Elasticsearch/Infrastructure.Elasticsearch.csproj", "BuildingBlocks/Infrastructure.Elasticsearch/"]
RUN dotnet restore "Services/Search/Search.API/Search.API.csproj"
COPY . .
WORKDIR "/src/Services/Search/Search.API"
RUN dotnet build "Search.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Search.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Search.API.dll"]