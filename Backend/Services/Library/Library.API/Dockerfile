#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Library/Library.API/Library.API.csproj", "Services/Library/Library.API/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
COPY ["BuildingBlocks/EventBus.Helper/EventBus.Helper.csproj", "BuildingBlocks/EventBus.Helper/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Infrastructure.MongoDb/Infrastructure.MongoDb.csproj", "BuildingBlocks/Infrastructure.MongoDb/"]
COPY ["Services/Library/Library.Domain/Library.Domain.csproj", "Services/Library/Library.Domain/"]
COPY ["Services/Library/Library.Infrastructure/Library.Infrastructure.csproj", "Services/Library/Library.Infrastructure/"]
RUN dotnet restore "Services/Library/Library.API/Library.API.csproj"
COPY . .
WORKDIR "/src/Services/Library/Library.API"
RUN dotnet build "Library.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Library.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.API.dll"]