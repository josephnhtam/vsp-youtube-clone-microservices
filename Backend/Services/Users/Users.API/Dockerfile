#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Users/Users.API/Users.API.csproj", "Services/Users/Users.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.Helper/EventBus.Helper.csproj", "BuildingBlocks/EventBus.Helper/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["Services/Storage/Storage.Shared/Storage.Shared.csproj", "Services/Storage/Storage.Shared/"]
COPY ["Services/Users/Users.Domain/Users.Domain.csproj", "Services/Users/Users.Domain/"]
COPY ["Services/Users/Users.Infrastructure/Users.Infrastructure.csproj", "Services/Users/Users.Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.MongoDb/Infrastructure.MongoDb.csproj", "BuildingBlocks/Infrastructure.MongoDb/"]
RUN dotnet restore "Services/Users/Users.API/Users.API.csproj"
COPY . .
WORKDIR "/src/Services/Users/Users.API"
RUN dotnet build "Users.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Users.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.API.dll"]