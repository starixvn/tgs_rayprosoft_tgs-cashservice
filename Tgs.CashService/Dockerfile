FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.CashService/Tgs.CashService.csproj", "Tgs.CashService/"]
COPY ["AgentWalletTransferTool/AgentWalletTransferTool.csproj", "AgentWalletTransferTool/"]
COPY ["Tgs.CashService.Client/Tgs.CashService.Client.csproj", "Tgs.CashService.Client/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
COPY ["Tgs.CashService.Core/Tgs.CashService.Core.csproj", "Tgs.CashService.Core/"]
COPY ["Cash.CommonModels/Cash.CommonModels.csproj", "Cash.CommonModels/"]
COPY ["Tgs.CashService.Repositories/Tgs.CashService.Repositories.csproj", "Tgs.CashService.Repositories/"]
RUN dotnet restore "./Tgs.CashService/./Tgs.CashService.csproj"
COPY . .
WORKDIR "/src/Tgs.CashService"
RUN dotnet build "./Tgs.CashService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.CashService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashService.dll"]