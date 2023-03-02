#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/History/History.API/History.API.csproj", "Services/History/History.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.Helper/EventBus.Helper.csproj", "BuildingBlocks/EventBus.Helper/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["Services/History/History.Domain/History.Domain.csproj", "Services/History/History.Domain/"]
COPY ["Services/History/History.Infrastructure/History.Infrastructure.csproj", "Services/History/History.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.Elasticsearch/Infrastructure.Elasticsearch.csproj", "BuildingBlocks/Infrastructure.Elasticsearch/"]
COPY ["BuildingBlocks/Infrastructure.MongoDb/Infrastructure.MongoDb.csproj", "BuildingBlocks/Infrastructure.MongoDb/"]
RUN dotnet restore "Services/History/History.API/History.API.csproj"
COPY . .
WORKDIR "/src/Services/History/History.API"
RUN dotnet build "History.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "History.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "History.API.dll"]