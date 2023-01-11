#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.CashService/Tgs.CashService.csproj", "Tgs.CashService/"]
COPY ["Tgs.CashService.Core/Tgs.CashService.Core.csproj", "Tgs.CashService.Core/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
COPY ["Tgs.CashService.ServiceRepositories/Tgs.CashService.ServiceRepositories.csproj", "Tgs.CashService.ServiceRepositories/"]
RUN dotnet restore "Tgs.CashService/Tgs.CashService.csproj"
COPY . .
WORKDIR "/src/Tgs.CashService"
RUN dotnet build "Tgs.CashService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.CashService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashService.dll"]