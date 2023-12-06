#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["WalletSanpshotTransferTool/WalletSanpshotTransferTool.csproj", "WalletSanpshotTransferTool/"]
COPY ["Tgs.CashService.Client/Tgs.CashService.Client.csproj", "Tgs.CashService.Client/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
RUN dotnet restore "./WalletSanpshotTransferTool/./WalletSanpshotTransferTool.csproj"
COPY . .
WORKDIR "/src/WalletSanpshotTransferTool"
RUN dotnet build "./WalletSanpshotTransferTool.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WalletSanpshotTransferTool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WalletSanpshotTransferTool.dll"]