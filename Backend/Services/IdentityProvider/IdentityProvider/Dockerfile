#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/IdentityProvider/IdentityProvider/IdentityProvider.csproj", "Services/IdentityProvider/IdentityProvider/"]
COPY ["BuildingBlocks/Infrastructure.EFCore/Infrastructure.EFCore.csproj", "BuildingBlocks/Infrastructure.EFCore/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Application/Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/SharedKernel/SharedKernel.csproj", "BuildingBlocks/SharedKernel/"]
RUN dotnet restore "Services/IdentityProvider/IdentityProvider/IdentityProvider.csproj"

COPY . .
WORKDIR "/src/Services/IdentityProvider/IdentityProvider"
RUN dotnet build "IdentityProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityProvider.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityProvider.dll"]