#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["AgentWalletTransferTool/AgentWalletTransferTool.csproj", "AgentWalletTransferTool/"]
COPY ["Tgs.CashService.Client/Tgs.CashService.Client.csproj", "Tgs.CashService.Client/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
RUN dotnet restore "AgentWalletTransferTool/AgentWalletTransferTool.csproj"
COPY . .
WORKDIR "/src/AgentWalletTransferTool"
RUN dotnet build "AgentWalletTransferTool.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "AgentWalletTransferTool.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentWalletTransferTool.dll"]