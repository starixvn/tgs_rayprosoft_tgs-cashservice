FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.CashQueryService/Tgs.CashQueryService.csproj", "Tgs.CashQueryService/"]
COPY ["CashQuery.Core/CashQuery.Core.csproj", "CashQuery.Core/"]
COPY ["CashQuery.Common/CashQuery.Common.csproj", "CashQuery.Common/"]
COPY ["Cash.CommonModels/Cash.CommonModels.csproj", "Cash.CommonModels/"]
COPY ["CashQuery.Repositories/CashQuery.Repositories.csproj", "CashQuery.Repositories/"]
RUN dotnet restore "./Tgs.CashQueryService/./Tgs.CashQueryService.csproj"
COPY . .
WORKDIR "/src/Tgs.CashQueryService"
RUN dotnet build "./Tgs.CashQueryService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.CashQueryService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashQueryService.dll"]