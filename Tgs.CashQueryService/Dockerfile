#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.CashQueryService/Tgs.CashQueryService.csproj", "Tgs.CashQueryService/"]
COPY ["CashQuery.Core/CashQuery.Core.csproj", "CashQuery.Core/"]
COPY ["CashQuery.Common/CashQuery.Common.csproj", "CashQuery.Common/"]
COPY ["Cash.CommonModels/Cash.CommonModels.csproj", "Cash.CommonModels/"]
COPY ["CashQuery.Repositories/CashQuery.Repositories.csproj", "CashQuery.Repositories/"]
RUN dotnet restore "Tgs.CashQueryService/Tgs.CashQueryService.csproj"
COPY . .
WORKDIR "/src/Tgs.CashQueryService"
RUN dotnet build "Tgs.CashQueryService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.CashQueryService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashQueryService.dll"]